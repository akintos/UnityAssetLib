using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;
using UnityAssetLib.Types;

namespace UnityAssetLib.ExternalTypes
{
    [UnitySerializable]
    public sealed class I2Loc_LanguageSource : MonoBehaviour
    {
        public byte NeverDestroy;
        public byte UserAgreesToHaveItOnTheScene;
        public byte UserAgreesToHaveItInsideThePluginsFolder;
        public byte GoogleLiveSyncIsUptoDate;

        public PPtr[] Assets;

        public string Google_WebServiceURL;
        public string Google_SpreadsheetKey;
        public string Google_SpreadsheetName;
        public string Google_LastUpdatedVersion;

        public int GoogleUpdateFrequency;
        public int GoogleInEditorCheckFrequency;
        public float GoogleUpdateDelay;

        public LanguageData[] mLanguages;

        public byte IgnoreDeviceLanguage;

        public int _AllowUnloadingLanguages;

        public TermData[] mTerms;

        public byte CaseInsensitiveTerms;
        public int OnMissingTranslation;
        public string mTerm_AppName;

        [UnitySerializable]
        public class LanguageData
        {
            public string Name;
            public string Code;
            public byte Flags;
        }

        [UnitySerializable]
        public class TermData
        {
            public string Term;
            public int TermType;
            public string Description;
            public string[] Languages;
            public byte[] Flags;
            public string[] Languages_Touch;
        }
    }
}
