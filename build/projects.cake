internal class BuildProjects
{
  private BuildProjects()
  {
  }

  public ICollection<SolutionProject> All { get; private set; }
  public ICollection<SolutionProject> NoneTests { get; private set; }
  public ICollection<SolutionProject> OnlyTests { get; private set; }

  public static Lazy<BuildProjects> Instance(ICakeContext context, string solution)
  {
    return new Lazy<BuildProjects>(() =>
    {
      var allProjects = context.ParseSolution($"src/{solution}.sln").Projects.ToList();

      return new BuildProjects
      {
        All = allProjects,
        NoneTests = allProjects.Where(project => !project.Name.EndsWith("Tests")).ToList(),
        OnlyTests = allProjects.Where(project => project.Name.EndsWith("Tests")).ToList()
      };
    });
  }
}
