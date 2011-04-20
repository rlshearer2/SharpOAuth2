﻿#region License
/* The MIT License
 * 
 * Copyright (c) 2011 Geoff Horsey
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using SharpOAuth2.Provider.Framework;

namespace SharpOAuth2.Provider.AuthorizationEndpoint.Inspectors
{
    public class SpecificationInspector : IContextInspector<IAuthorizationContext>
    {
        #region IAuthorizationContextInspector Members

        public void Inspect(IAuthorizationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ResponseType))
                throw Errors.InvalidRequestException(context, Parameters.ResponseType);

            if (context.Client == null || string.IsNullOrWhiteSpace(context.Client.ClientId))
                throw Errors.InvalidRequestException(context, Parameters.ClientId);

            if (context.RedirectUri == null)
                throw Errors.InvalidRequestException(context, Parameters.RedirectUri);

            if (context.ResponseType != Parameters.ResponseTypeValues.AccessToken && context.ResponseType != Parameters.ResponseTypeValues.AuthorizationCode)
                throw Errors.UnsupportedResponseType(context, context.ResponseType);

        }

        #endregion
    }
}
