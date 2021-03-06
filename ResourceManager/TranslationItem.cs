﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ResourceManager.Translation
{
    public enum TranslationType
    {
        [XmlEnum(Name = "translated")]
        Translated,
        [XmlEnum(Name = "unfinished")]
        Unfinished,
        [XmlEnum(Name = "obsolete")]
        Obsolete,
        [XmlEnum(Name = "new")]
        New
    }

    [DebuggerDisplay("{name}.{property}={value}")]
    public class TranslationItem : IComparable<TranslationItem>
    {
        public TranslationItem()
        {

        }

        public TranslationItem(string name, string property, string value)
        {
            this.name = name;
            this.property = property;
            this.value = value;
        }

        public TranslationItem(string name, string property, string source, string value)
        {
            this.name = name;
            this.property = property;
            this.source = source;
            this.value = value;
        }

        private string name;
        [XmlAttribute]
        public string Name 
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private string property;
        [XmlAttribute]
        public string Property
        {
            get
            {
                return property;
            }
            set
            {
                property = value;
            }
        }

        private TranslationType status;
        [XmlAttribute("type"), DefaultValue(0)]
        public TranslationType Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                this.source = value;
            }
        }

        private string value;
        public string Value 
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public int CompareTo(TranslationItem other)
        {
            int val = Name.CompareTo(other.Name);
            if (val == 0) val = Property.CompareTo(other.Property);
            return val;
        }
    }
}
