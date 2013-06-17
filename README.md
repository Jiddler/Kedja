Kedja - Chain your tasks
=====

Kedja is a fluently configured workflow engine.

What does Kedja mean?
------
Kedja comes from the swedish word and means Chain.

Example
------
            var workflow = new WorkFlow<SignInContext>()
                .AddStep<VerifyUsernameExistande, bool>(b => b.When(false).AddStep<ErrorMessage>().Stop())
                .AddStep<VerifyCredentials, bool>(b => {
                    b.When(false).AddStep<ErrorMessage>();
                    b.Otherwise()
                        .AddStep<PutInSession>()
                        .AddStep<LogToDatabase>();
                });

            workflow.Execute();

See unit tests for many more examples.
