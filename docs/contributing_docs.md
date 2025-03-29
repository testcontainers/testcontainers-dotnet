# Contributing to documentation

The Testcontainers for .NET documentation is a static site built with [MkDocs](https://www.mkdocs.org/).
We use the [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/) theme, which offers a number of useful extensions to MkDocs.

In addition we use a [custom plugin](https://github.com/rnorth/mkdocs-codeinclude-plugin) for inclusion of code snippets.

We publish our documentation using Netlify.

## Previewing rendered content

### Using Python locally

* Ensure that you have Python 3.8.0 or higher.
* Set up a virtualenv and run `pip install -r requirements.txt` in the `testcontainers-dotnet` root directory.
* Once Python dependencies have been installed, run `mkdocs serve` to start a local auto-updating MkDocs server.

### Using Docker

The root of the project contains a `compose.yml` file. Simply run `docker compose up` and access the docs at: `http://localhost:8000`.

### PR preview deployments

Note that documentation for pull requests will automatically be published by Netlify as 'deploy preview'.
These deployment previews can be accessed via the `netlify/testcontainers-dotnet/deploy-preview` check that appears for each pull request.
