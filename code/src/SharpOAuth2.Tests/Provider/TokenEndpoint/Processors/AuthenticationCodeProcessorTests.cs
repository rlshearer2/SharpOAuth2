﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpOAuth2.Provider.TokenEndpoint;
using SharpOAuth2.Provider.Services;
using Moq;
using SharpOAuth2.Provider.TokenEndpoint.Processor;

namespace SharpOAuth2.Tests.Provider.TokenEndpoint.Processors
{
    [TestFixture]
    public class AuthenticationCodeProcessorTests
    {
        [Test]
        public void TestSatisfiedByMethod()
        {
            TokenContext context = new TokenContext();
            context.GrantType = Parameters.GrantTypeValues.AuthorizationCode;

            AuthenticationCodeProcessor processor = new AuthenticationCodeProcessor(new Mock<IServiceFactory>().Object);

            Assert.IsTrue(processor.IsSatisfiedBy(context));
        }

        [Test]
        public void TestProcessingValidContext()
        {
            TokenContext context = new TokenContext()
            {
                Client = new ClientBase{ ClientId="321", ClientSecret="secret"},
                AuthorizationCode = "123",
                GrantType = Parameters.GrantTypeValues.AuthorizationCode,
                RedirectUri = new Uri("http://www.mysites.com/callback"),
                Scope = new string[] { "create", "delete" }
            };

            Mock<ITokenService> mckTokenService = new Mock<ITokenService>();
            mckTokenService.Setup(x => x.FindAuthorizationGrant("123")).Returns(new AuthorizationGrantBase { Client = new ClientBase { ClientId = "321", ClientSecret = "secret" } });
            mckTokenService.Setup(x => x.AuthorizationGrantIsValid(It.IsAny<AuthorizationGrantBase>())).Returns(true);
            mckTokenService.Setup(x => x.SetAccessToken(context));
            Mock<IClientService> mckClientService = new Mock<IClientService>();
            mckClientService.Setup(x => x.FindClient("321")).Returns(new ClientBase { ClientSecret = "secret", ClientId = "321" });
            mckClientService.Setup(x => x.AuthenticateClient(context)).Returns(true);

            Mock<IServiceFactory> mckFactory = new Mock<IServiceFactory>();
            mckFactory.SetupGet(x => x.ClientService).Returns(mckClientService.Object);
            mckFactory.SetupGet(x => x.TokenService).Returns(mckTokenService.Object);

            AuthenticationCodeProcessor processor = new AuthenticationCodeProcessor(mckFactory.Object);

            processor.Process(context);

            mckFactory.VerifyAll();
            mckClientService.VerifyAll();
            mckTokenService.VerifyAll();
        }
    }
}
