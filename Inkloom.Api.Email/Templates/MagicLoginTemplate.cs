using System;
using System.Collections.Generic;

namespace Inkloom.Api.Email.Templates
{
    public class MagicLoginTemplate : Template
    {
        public MagicLoginTemplate(string username, string token, int validity) : base(new ITemplateOptions()
        {
            preCtaParagraphs = new string[]{
                    $"Hello {username}",
                    $"Please click this button to login to inkloom:"
                },
            ctaLink = $"https://inkloom.com/magic-login?token={token}",
            ctaTitle = "Login Now",
            postCtaParagraphs = new string[]{
                    "Button not working? Copy and paste this link in browser:",
                    $"https://inkloom.com/magic-login?token={token}",
                    "",
                    $"This link will work only for {validity} minutes.",
                    "If you didn't request a login link, please reset password to protect your account by visiting https://inkloom.com/reset-password."
                 }
        })
        {

        }
    }
}