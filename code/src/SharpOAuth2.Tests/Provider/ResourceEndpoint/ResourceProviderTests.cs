﻿using System;
using System.Collections.Specialized;
using CuttingEdge.ServiceLocation;
using Microsoft.Practices.ServiceLocation;
using Moq;
using NUnit.Framework;
using SharpOAuth2.Framework;
using SharpOAuth2.Provider.Domain;
using SharpOAuth2.Provider.Framework;
using SharpOAuth2.Provider.ResourceEndpoint;
using SharpOAuth2.Provider.Services;
using SharpOAuth2.Provider.Utility;

namespace SharpOAuth2.Tests.Provider.ResourceEndpoint
{
    [TestFixture]
    public class ResourceProviderTests
    {
        [Test]
        public void TestAccessingProtectedResource()
        {
            ResourceContext context = new ResourceContext();
            context.Headers = new NameValueCollection();
            context.Headers["Authorization"] = "bearer my-token";

            Mock<ContextProcessor<IResourceContext>> mckProvider = new Mock<ContextProcessor<IResourceContext>>(new Mock<IServiceFactory>().Object);
            mckProvider.Setup(x => x.IsSatisfiedBy(context)).Returns(true);
            mckProvider.Setup(x => x.Process(context)).Callback(() => { context.Token = new AccessTokenBase(); });

            SimpleServiceLocator container = new SimpleServiceLocator();
            container.RegisterAll<ContextProcessor<IResourceContext>>(mckProvider.Object);

            ServiceLocator.SetLocatorProvider(() => container);

            ResourceProvider provider = new ResourceProvider();

            provider.AccessProtectedResource(context);


            mckProvider.Verify();

        }


        [Test, ExpectedException(typeof(OAuthFatalException))]
        public void TestAccessingResourceUnhandledByProcessor()
        {
            ResourceContext context = new ResourceContext();
            context.Headers = new NameValueCollection();
            context.Headers["Authorization"] = "bearer my-token";

            Mock<ContextProcessor<IResourceContext>> mckProvider = new Mock<ContextProcessor<IResourceContext>>(new Mock<IServiceFactory>().Object);
            mckProvider.Setup(x => x.IsSatisfiedBy(context)).Returns(false);

            SimpleServiceLocator container = new SimpleServiceLocator();
            container.RegisterAll<ContextProcessor<IResourceContext>>(mckProvider.Object);

            ServiceLocator.SetLocatorProvider(() => container);

            ResourceProvider provider = new ResourceProvider();

            provider.AccessProtectedResource(context);


            mckProvider.Verify();

        }

        [Test]
        public void TestAccessingNullTokenResource()
        {
            ResourceContext context = new ResourceContext();
            context.Headers = new NameValueCollection();
            context.Headers["Authorization"] = "bearer my-token";

            Mock<ContextProcessor<IResourceContext>> mckProvider = new Mock<ContextProcessor<IResourceContext>>(new Mock<IServiceFactory>().Object);
            mckProvider.Setup(x => x.IsSatisfiedBy(context)).Returns(true);
            mckProvider.Setup(x => x.Process(context));

            SimpleServiceLocator container = new SimpleServiceLocator();
            container.RegisterAll<ContextProcessor<IResourceContext>>(mckProvider.Object);

            ServiceLocator.SetLocatorProvider(() => container);

            ResourceProvider provider = new ResourceProvider();

            CommonErrorAssert(context, provider, Parameters.ErrorParameters.ErrorValues.InvalidToken);

            mckProvider.Verify();
        }

        [Test]
        public void TestAccessingExpiredTokenResource()
        {
            ResourceContext context = new ResourceContext();
            context.Headers = new NameValueCollection();
            context.Headers["Authorization"] = "bearer my-token";

			IAccessToken expiredToken = new AccessTokenBase
            {
                Token = "my-token",
                ExpiresIn = 2,
                IssuedOn = DateTime.Now.AddMinutes(-1).ToEpoch()
            };
            Mock<ContextProcessor<IResourceContext>> mckProvider = new Mock<ContextProcessor<IResourceContext>>(new Mock<IServiceFactory>().Object);
            mckProvider.Setup(x => x.IsSatisfiedBy(context)).Returns(true);
            mckProvider.Setup(x => x.Process(context)).Callback(() => context.Token = expiredToken);

            SimpleServiceLocator container = new SimpleServiceLocator();
            container.RegisterAll<ContextProcessor<IResourceContext>>(mckProvider.Object);

            ServiceLocator.SetLocatorProvider(() => container);

            ResourceProvider provider = new ResourceProvider();

            CommonErrorAssert(context, provider, Parameters.ErrorParameters.ErrorValues.InvalidToken);

            mckProvider.Verify();
        }
        [Test]
        public void TestValidScope()
        {
            ResourceContext context = new ResourceContext();
            context.Token = new AccessTokenBase { Scope = new[] { "create" } };

            ResourceProvider provider = new ResourceProvider();

            provider.ValidateScope(context, new string[] { "CREATE" });
        }

        [Test]
        public void TestInvalidMissingTokenScope()
        {
            ResourceContext context = new ResourceContext();

            ResourceProvider provider = new ResourceProvider();

            try
            {
                provider.ValidateScope(context, new string[] { "delete" });
                Assert.Fail("No exception was thrown");
            }
            catch (OAuthErrorResponseException<IResourceContext> x)
            {
                Assert.AreEqual(Parameters.ErrorParameters.ErrorValues.InvalidToken, x.Error);
                Assert.AreEqual(401, x.HttpStatusCode);
            }
            catch (Exception x)
            {
                Assert.Fail("Unexpected exception: " + x.Message);
            }
        }

        [Test]
        public void TestInvalidScope()
        {
            ResourceContext context = new ResourceContext();
            context.Token = new AccessTokenBase { Scope = new[] { "create" } };

            ResourceProvider provider = new ResourceProvider();

            try
            {
                provider.ValidateScope(context, new string[] { "delete" });
                Assert.Fail("No exception was thrown");
            }
            catch (OAuthErrorResponseException<IResourceContext> x)
            {
                Assert.AreEqual(Parameters.ErrorParameters.ErrorValues.InsufficientScope, x.Error);
                Assert.AreEqual(403, x.HttpStatusCode);
            }
            catch (Exception x)
            {
                Assert.Fail("Unexpected exception: " + x.Message);
            }
        }


        private static void CommonErrorAssert(ResourceContext context, ResourceProvider provider, string error)
        {
            try
            {
                provider.AccessProtectedResource(context);
                Assert.Fail("No exception was thrown");
            }
            catch (OAuthErrorResponseException<IResourceContext> x)
            {
                Assert.AreEqual(error, x.Error);
            }
            catch (Exception x)
            {
                Assert.Fail("Unexpected exception was thrown:" + x.Message);
            }
        }
    }
}
