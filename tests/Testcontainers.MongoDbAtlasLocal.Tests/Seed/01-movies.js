db.movies.insertMany([
  { title: "The Matrix" },
  { title: "Back to the Future" },
  { title: "The Matrix Reloaded" },
]);

db.movies.createSearchIndex("default", { mappings: { dynamic: true } });
