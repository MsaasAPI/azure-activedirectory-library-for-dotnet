﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AdalAndroidTestApp
{
    [Activity(Label = "AdalAndroidTestApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string clientId = "d3590ed6-52b3-4102-aeff-aad2292ab01c";
        private const string redirectUri = "urn:ietf:wg:oauth:2.0:oob";
        private const string resource1 = "https://graph.windows.net";
        private const string resource2 = "https://graph.microsoft.com";
        readonly string user = "<USER>";

        private UITextView accessTokenTextView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button acquireTokenInteractiveButton = FindViewById<Button>(Resource.Id.acquireTokenInteractiveButton);
            acquireTokenInteractiveButton.Click += acquireTokenInteractiveButton_Click;

            Button acquireTokenSilentButton = FindViewById<Button>(Resource.Id.acquireTokenSilentButton);
            acquireTokenSilentButton.Click += acquireTokenSilentButton_Click;

            Button clearCacheButton = FindViewById<Button>(Resource.Id.clearCacheButton);
            clearCacheButton.Click += clearCacheButton_Click;

            this.accessTokenTextView = new UITextView(this, FindViewById<TextView>(Resource.Id.accessTokenTextView));

            EditText email = FindViewById<EditText>(Resource.Id.email);
            email.Text = user;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode,
                data);
        }

        private async void acquireTokenSilentButton_Click(object sender, EventArgs e)
        {
            this.accessTokenTextView.Text = string.Empty;
            EditText email = FindViewById<EditText>(Resource.Id.email);
            string value = null;
            try
            {
                AuthenticationContext ctx = new AuthenticationContext("https://login.microsoftonline.com/common");
                AuthenticationResult result = await ctx
                    .AcquireTokenSilentAsync(resource1, clientId, UserIdentifier.AnyUser,
                        new PlatformParameters(this, false)).ConfigureAwait(false);
                value = result.AccessToken;
            }
            catch (Java.Lang.Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                value = exc.Message;
            }

            this.accessTokenTextView.Text = value;
        }

        private async void acquireTokenInteractiveButton_Click(object sender, EventArgs e)
        {
            this.accessTokenTextView.Text = string.Empty;
            AuthenticationContext ctx = new AuthenticationContext("https://login.microsoftonline.com/common/");
            EditText email = FindViewById<EditText>(Resource.Id.email);
            string value = null;
            try
            {
                AuthenticationResult result = await ctx
                    .AcquireTokenAsync(resource2, clientId, new Uri(redirectUri),
                        new PlatformParameters(this, false)).ConfigureAwait(false);
                value = result.AccessToken;
            }
            catch (Java.Lang.Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                value = exc.Message;
            }

            this.accessTokenTextView.Text = value;
        }

        private async void clearCacheButton_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                TokenCache.DefaultShared.Clear();
                this.accessTokenTextView.Text = "Cache cleared";
            });
        }
    }
}