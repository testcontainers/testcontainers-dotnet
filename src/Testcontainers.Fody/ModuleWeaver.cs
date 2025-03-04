namespace Testcontainers.Fody;

public sealed class ModuleWeaver : BaseModuleWeaver
{
    static ModuleWeaver()
    {
        // Debugger.Launch();
    }

    public ModuleWeaver()
    {
        // Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    public override void Execute()
    {
        foreach (var type in ModuleDefinition.Types)
        {
            var containerFields = type.Fields.Where(field => field.CustomAttributes.Any(Matches<AutoStartContainerAttribute>)).ToList();

            if (containerFields.Count == 0)
            {
                continue;
            }

            var constructors = type.Methods.Where(method => method.IsConstructor && !method.IsStatic).ToList();

            if (constructors.Count == 0)
            {
                continue;
            }

            var constructor = constructors[0];

            var containerField = containerFields[0];

            // Get relevant types
            var threadType = ModuleDefinition.ImportReference(typeof(Thread));

            // Get method references
            var startAsyncMethod = ModuleDefinition.ImportReference(typeof(IContainer).GetMethod(nameof(IContainer.StartAsync), new[] { typeof(CancellationToken) }));
            var cancellationTokenGetNoneMethod = ModuleDefinition.ImportReference(typeof(CancellationToken).GetMethod("get_None", Type.EmptyTypes));
            var waitMethod = ModuleDefinition.ImportReference(typeof(Task).GetMethod(nameof(Task.Wait), Type.EmptyTypes));
            var threadStartCtor = ModuleDefinition.ImportReference(typeof(ThreadStart).GetConstructor(new[] { typeof(object), typeof(IntPtr) }));
            var threadCtor = ModuleDefinition.ImportReference(typeof(Thread).GetConstructor(new[] { typeof(ThreadStart) }));
            var threadStartMethod = ModuleDefinition.ImportReference(typeof(Thread).GetMethod(nameof(Thread.Start), Type.EmptyTypes));
            var threadJoinMethod = ModuleDefinition.ImportReference(typeof(Thread).GetMethod(nameof(Thread.Join), Type.EmptyTypes));

            // Create IL Processor
            var ilProcessor = constructor.Body.GetILProcessor();
            var firstInstruction = constructor.Body.Instructions.Last();

            // Create variable for Thread
            var threadVar = new VariableDefinition(threadType);
            constructor.Body.Variables.Add(threadVar);

            // Inject IL instructions
            var instructions = new List<Instruction>
            {
                // Load `this`
                Instruction.Create(OpCodes.Ldarg_0),

                // Load `_container` field
                Instruction.Create(OpCodes.Ldfld, containerField),

                // Create a CancellationToken
                Instruction.Create(OpCodes.Call, cancellationTokenGetNoneMethod),

                // Call _container.StartAsync(cancellationToken)
                Instruction.Create(OpCodes.Callvirt, startAsyncMethod),

                // Get function pointer for Task.Wait()
                Instruction.Create(OpCodes.Ldftn, waitMethod),

                // Create new ThreadStart delegate
                Instruction.Create(OpCodes.Newobj, threadStartCtor),

                // Create new Thread object
                Instruction.Create(OpCodes.Newobj, threadCtor),

                // Store in local variable (Thread thread = new Thread(...))
                Instruction.Create(OpCodes.Stloc_S, threadVar),

                // Call thread.Start()
                Instruction.Create(OpCodes.Ldloc_S, threadVar),
                Instruction.Create(OpCodes.Callvirt, threadStartMethod),

                // Call thread.Join()
                Instruction.Create(OpCodes.Ldloc_S, threadVar),
                Instruction.Create(OpCodes.Callvirt, threadJoinMethod),
            };

            // Insert IL instructions at the beginning of the constructor
            foreach (var instruction in instructions)
            {
                ilProcessor.InsertBefore(firstInstruction, instruction);
            }
        }
    }

    private static bool Matches<TCustomAttributeEntity>(CustomAttribute customAttribute)
    {
        return typeof(TCustomAttributeEntity).FullName!.Equals(customAttribute.AttributeType.FullName, StringComparison.Ordinal);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }
}