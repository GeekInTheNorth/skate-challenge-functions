// --------------------------------------------------------------------------------------------------------------------
// <copyright company="twentysix" file="IGravatarResolver.cs">
// Copyright (c) twentysix.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AzureFunctionTest
{
    public interface IGravatarResolver
    {
        string GetGravatarUrl(string emailAddress);
    }
}