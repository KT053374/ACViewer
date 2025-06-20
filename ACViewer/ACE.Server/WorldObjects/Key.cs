using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;

namespace ACE.Server.WorldObjects
{
    public class Key : WorldObject
    {
        /// <summary>
        /// A new biota be created taking all of its values from weenie.
        /// </summary>
        public Key(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
        {
            SetEphemeralValues();
        }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// </summary>
        public Key(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
            // These shoudl come from the weenie. After confirmation, remove these
            //KeyCode = AceObject.KeyCode ?? "";
            //Structure = AceObject.Structure ?? AceObject.MaxStructure;

            if (Structure > MaxStructure)
                Structure = MaxStructure;
        }

        public string KeyCode
        {
            get => GetProperty(PropertyString.KeyCode);
            set { if (value == null) RemoveProperty(PropertyString.KeyCode); else SetProperty(PropertyString.KeyCode, value); }
        }

        public bool OpensAnyLock
        {
            get => GetProperty(PropertyBool.OpensAnyLock) ?? false;
            set { if (!value) RemoveProperty(PropertyBool.OpensAnyLock); else SetProperty(PropertyBool.OpensAnyLock, value); }
        }
    }
}
