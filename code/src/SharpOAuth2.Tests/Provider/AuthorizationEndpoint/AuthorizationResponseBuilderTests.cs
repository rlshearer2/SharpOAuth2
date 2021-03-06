﻿using System;
using NUnit.Framework;
using SharpOAuth2.Framework;
using SharpOAuth2.Provider.AuthorizationEndpoint;
using SharpOAuth2.Provider.Domain;
using SharpOAuth2.Provider.Framework;

namespace SharpOAuth2.Tests.Provider.AuthorizationEndpoint
{
    [TestFixture]
    public class AuthorizationResponseBuilderTests
    {
        private IAuthorizationContext MakeErrorAuthorizationContext(string responseType)
        {
            AuthorizationContext ctx = new AuthorizationContext
            {
                ResponseType = responseType,
                Error = new ErrorResponse
                {
                    Error = Parameters.ErrorParameters.ErrorValues.AccessDenied,
                    ErrorDescription = "You do not have access"
                },
                RedirectUri = new Uri("http://www.mysite.com/callback?param=maintain")
            };
            return ctx;
        }

        private IAuthorizationContext MakeAuthorizatonCodeContext(string responseType)
        {
            AuthorizationContext context = new AuthorizationContext
            {
                AuthorizationGrant = new AuthorizationGrantBase
                {
                    //Scope = new string[] { "create", "delete" },
                    Code = "special-token-value"
                },
                Token = new AccessTokenBase
                {
                    Token = "access-token",
                    TokenType="bearer",
                    ExpiresIn = 123,
                    Scope = new string[]{ "read"}
                },
                IsApproved = true,
                RedirectUri = new Uri("http://www.mysite.com/callback?param=maintain"),
                ResourceOwnerUsername = "1234",
                ResponseType = responseType,
                Scope = new string[] { "create", "delete" },
                State = "special"
            };

            return context;
        }

        [Test]
        public void CreateAuthorizationResponseForRedirectFlow()
        {
            IAuthorizationContext context = MakeAuthorizatonCodeContext(Parameters.ResponseTypeValues.AuthorizationCode);
            IAuthorizationResponseBuilder builder = new AuthorizationResponseBuilder();
            context.State = "";
            Uri result = builder.CreateResponse(context);

            Assert.AreEqual("http://www.mysite.com/callback?param=maintain&code=special-token-value", result.AbsoluteUri);
        }

        [Test]
        public void CreateErrorAuthorizationResponse()
        {
            IAuthorizationContext context = MakeErrorAuthorizationContext(Parameters.ResponseTypeValues.AuthorizationCode);
            context.Error.ErrorDescription = string.Empty;
            
            IAuthorizationResponseBuilder builder = new AuthorizationResponseBuilder();

            Uri result = builder.CreateResponse(context);

            Assert.AreEqual("http://www.mysite.com/callback?param=maintain&error=access_denied", result.AbsoluteUri);
        }

        [Test]
        public void CreateTokenAuthorizationResponseForImplicit()
        {
            IAuthorizationContext context = MakeAuthorizatonCodeContext(Parameters.ResponseTypeValues.AccessToken);
            IAuthorizationResponseBuilder builder = new AuthorizationResponseBuilder();

            Uri result = builder.CreateResponse(context);

            Assert.AreEqual("http://www.mysite.com/callback?param=maintain#access_token=access-token&expires_in=123&token_type=bearer&scope=read&state=special", result.AbsoluteUri);
        }

        [Test]
        public void CreateErrorTokenAuthorizationResponse()
        {
            IAuthorizationContext context = MakeErrorAuthorizationContext(Parameters.ResponseTypeValues.AccessToken);
            IAuthorizationResponseBuilder builder = new AuthorizationResponseBuilder();

            Uri result = builder.CreateResponse(context);

            Assert.AreEqual("http://www.mysite.com/callback?param=maintain#error=access_denied&error_description=You%20do%20not%20have%20access", result.AbsoluteUri);
        }
    }
}
