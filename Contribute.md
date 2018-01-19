# Contributing to PowerShapeAndPowerMillAPI

We would love for you to contribute to API and help make it even better than it is today! As a contributor, here are the guidelines we would like you to follow:

# Contributor Code of Conduct

As contributors and maintainers of the API project, we pledge to respect everyone who contributes by posting issues, updating documentation, submitting pull requests, providing feedback in comments, and any other activities.

Communication through any of API's channels (GitHub, forum, IRC, mailing lists, Google+, Twitter, etc.) must be constructive and never resort to personal attacks, trolling, public or private harassment, insults, or other unprofessional conduct.

We promise to extend courtesy and respect to everyone involved in this project regardless of gender, gender identity, sexual orientation, disability, age, race, ethnicity, religion, or level of experience. We expect anyone contributing to the project to do the same.

If any member of the community violates this code of conduct, the maintainers of the project may take action, removing issues, comments, and PRs or blocking accounts as deemed appropriate.

If you are subject to or witness unacceptable behavior, or have any other concerns, please contact us directly.


Pull Request:

1. Before submitting the PR, please review this page for guidelines
2. PSPMAPI Team will meet weekly to review PRs found on Github (Issues will be handled separately)
3. Will be reviewed from oldest to newest
4. If a reviewed PR requires changes by the owner, the owner of the PR has 30 days to respond. If the PR has seen no activity by the next session, it will be either closed by the team or depending on its utility will be taken over by someone on the team
5. Should have a clear propose in order to be considered for review. What is the issue and how it is fixed? What is the new feature and is it implemented?
6. The level of testing this PR includes is appropriate
7. Should pass all existing tests
8. Is well documented 
9. User facing strings, if any, are extracted into `*.resx` files 
10. Snapshot of UI changes, if any



## Filing issues

When filing an issue, make sure to follow the following steps:

1. Which version of PSPMAPI are you using?
2. Which operating system are you using?
3. What did you do?
4. What did you expect to see?
5. What did you see instead?
6. If helpful, include a screenshot. Annotate the screenshot for clarity.

General questions about using PSPMAPI should be submitted to [the forum at PSPMAPI](https://forums.autodesk.com/t5/powershape-and-powermill-api/bd-p/298)

## Contributing code

Before submitting a Pull Request, please see Pull Request steps above.

Unless otherwise noted, the PSPMAPI source files are distributed under the MIT License.

## Contribution "Bar"


The PSPMAPI team will merge changes that make it easier for customers to use PSPMAPI.

The PSPMAPI team will not merge changes that have narrowly-defined benefits. Contributions must also satisfy the other published guidelines defined in this document.

## DOs and DON'Ts

Please do:

* **DO** follow our coding standards and naming standards
* **DO** include unit tests when adding new features. When fixing bugs, start with adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up it's often better to create new issue than to side track the discussion.
* **DO** blog and tweet (or whatever) about your contributions, frequently!

Please do not:

* **DON'T** surprise us with big pull requests. Instead, file an issue and start a discussion so we can agree on a direction before you invest a large amount of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to PSPMAPI, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
* **DON'T** add API additions without filing an issue and discussing with us first.

## Managed Code Compatibility


Contributions must maintain API backwards compatibility following semantic versioning. Contributions that include breaking changes will be rejected. Please file an issue to discuss your idea or change if you believe that it may affect managed code compatibility.

## Commit Messages


Please include a brief description of the change you made.  If it is based off an issue then mention this reference at the beginning of the commit message.

Also do your best to factor commits appropriately, not too large with unrelated things in the same commit, and not too small with the same small change applied N times in N different commits.

## Notes

This guide was based off of [DotNet Core Contributing Guide](https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/contributing.md)


