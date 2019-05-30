using System;

namespace AskodOnline.Editor.Models
{
    public class BrowserModel : BaseModel
    {
        public BrowserModel(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }

        public string Version { get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BrowserModel other = (BrowserModel)obj;
            return Name == other.Name && Version == other.Version;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0) * 397) ^ (Version != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Version) : 0);
            }
        }

        public static bool operator ==(BrowserModel userAgent1, BrowserModel userAgent2)
        {
            if (object.ReferenceEquals(userAgent1, null))
            {
                return object.ReferenceEquals(userAgent2, null);
            }

            return userAgent1.Equals(userAgent2);
        }

        public static bool operator !=(BrowserModel userAgent1, BrowserModel userAgent2)
        {
            if (object.ReferenceEquals(userAgent1, null))
            {
                return !object.ReferenceEquals(userAgent2, null);
            }

            return !userAgent1.Equals(userAgent2);
        }
    }
}