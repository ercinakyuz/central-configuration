﻿using System;

namespace CentralConfiguration.Model
{
    public class ConfigurationDto : BaseDto, IEquatable<ConfigurationDto>
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }

        public bool Equals(ConfigurationDto other)
        {
            if (other is null)
                return false;

            return Key.Equals(other.Key) && Value.Equals(other.Value) && Type.Equals(other.Type);
        }

        public override bool Equals(object obj) => Equals(obj as ConfigurationDto);
        public override int GetHashCode() => (Key, Value, Type).GetHashCode();
    }
}
