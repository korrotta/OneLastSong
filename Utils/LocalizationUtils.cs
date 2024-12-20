﻿using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WinUI3Localizer;

namespace OneLastSong.Utils
{
    public class LocalizationUtils
    {
        // Utils string for retrieving the current language
        public static readonly string TEST_STRING = "Test_String";
        public static readonly string LOGIN_STRING = "Login_String";
        public static readonly string SIGN_IN_FAIL_STRING= "SignInFail_String";
        public static readonly string FEATURE_NOT_IMPLEMENTED_STRING = "FeatureNotImplemented_String";
        public static readonly string INFO_STRING = "Info_String";
        public static readonly string SUCCESS_STRING = "Success_String";
        public static readonly string WARNING_STRING = "Warning_String";
        public static readonly string ERROR_STRING = "Error_String";
        public static readonly string SIGN_UP_SUCCESS_STRING = "SignUpSuccess_String";
        public static readonly string SIGN_IN_SUCCESS_STRING = "SignInSuccess_String";
        public static readonly string FORM_CONTAINING_ERROR_STRING = "FormContainingError_String";
        public static readonly string Ok_Button_Content = "Ok_Button.Content";
        public static readonly string Cancel_Button_Content = "Cancel_Button.Content";
        public static readonly string NOT_SIGNED_IN_ERROR_STRING = "NotSignedInError_String";

        public static async Task InitializeLocalizer()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder stringsFolder = await localFolder.CreateFolderAsync(
              "Strings",
               CreationCollisionOption.OpenIfExists);

            string resourceFileName = "Resources.resw";
            await CreateStringResourceFileIfNotExists(stringsFolder, "en", resourceFileName);
            await CreateStringResourceFileIfNotExists(stringsFolder, "vn", "Resources.vn.resw");

            ILocalizer localizer = await new LocalizerBuilder()
                .AddStringResourcesFolderForLanguageDictionaries(stringsFolder.Path)
                .SetOptions(options =>
                {
                    options.DefaultLanguage = "en";
                })
                .Build();

            // Debug: Check if dictionaries are loaded
            var dictionaries = localizer.GetLanguageDictionaries();
            foreach (var dictionary in dictionaries)
            {
                if (dictionary.GetItemsCount() > 0)
                {
                    LogUtils.Debug($"Language: {dictionary.Language}, Keys: {string.Join(", ", dictionary.GetItems())}");
                }
            }
        }

        internal static string GetString(string v)
        {
            return Localizer.Get().GetLocalizedString(v);
        }

        private static async Task CreateStringResourceFileIfNotExists(StorageFolder stringsFolder, string language, string resourceFileName)
        {
            StorageFolder languageFolder = await stringsFolder.CreateFolderAsync(
                language,
                CreationCollisionOption.OpenIfExists);

            StorageFile existingFile = await languageFolder.TryGetItemAsync(resourceFileName) as StorageFile;
            string resourceFilePath = System.IO.Path.Combine(stringsFolder.Name, language, resourceFileName);
            StorageFile sourceFile = await LoadStringResourcesFileFromAppResource(resourceFilePath);

            bool shouldCopy = false;

            if (existingFile is null)
            {
                shouldCopy = true;
            }
            else
            {
                var sourceProperties = await sourceFile.GetBasicPropertiesAsync();
                var destinationProperties = await existingFile.GetBasicPropertiesAsync();

                if (sourceProperties.DateModified > destinationProperties.DateModified)
                {
                    shouldCopy = true;
                }
            }

            if (shouldCopy)
            {
                _ = await sourceFile.CopyAsync(languageFolder, "Resources.resw", NameCollisionOption.ReplaceExisting);
            }
        }

        private static async Task<StorageFile> LoadStringResourcesFileFromAppResource(string filePath)
        {
            Uri resourcesFileUri = new($"ms-appx:///{filePath}");
            try
            {
                return await StorageFile.GetFileFromApplicationUriAsync(resourcesFileUri);
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"When loading {resourcesFileUri.ToString}: {ex.Message}");
                LogUtils.Debug($"Exception: {ex.Message}");
                string message = $"Failed to load string resources file from {resourcesFileUri.ToString()}";
                throw new Exception(message, ex);
            }
        }
    }
}
