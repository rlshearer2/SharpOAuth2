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

using SharpOAuth2.Provider.AuthorizationEndpoint;
using SharpOAuth2.Provider.Domain;
using SharpOAuth2.Provider.Framework;

namespace SharpOAuth2.Provider.Services
{
    public interface ITokenService
    {
        AuthorizationGrantBase IssueAuthorizationGrant(IAuthorizationContext context);
        IToken IssueAccessToken(string resourceOwnerUsername);
        IToken IssueAccessToken(ClientBase client);
        IToken IssueAccessToken(string refreshToken, ClientBase client);
        IToken IssueAccessToken(AuthorizationGrantBase grant);

        void ApproveAuthorizationGrant(AuthorizationGrantBase authorizationGrant, bool isApproved);

        AuthorizationGrantBase FindAuthorizationGrant(string authorizationCode);
        AccessTokenBase FindToken(string token);
        bool ValidateRefreshTokenForClient(string refreshToken, ClientBase client);
        bool ValidateRedirectUri(IOAuthContext context, AuthorizationGrantBase grant);

        void ConsumeGrant(AuthorizationGrantBase grant);
    }
}