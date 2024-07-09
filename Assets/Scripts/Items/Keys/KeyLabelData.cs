using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

namespace Items.Keys
{
    [CreateAssetMenu]
    public class KeyLabelData : ScriptableObject
    {
        [SerializeField] private LabelColor _deffaultColor;

        [SerializeField] private SerializedDictionary<DoorKeyType, LabelColor> _keyLabelColors = new();

        public Color GetlColor(DoorKeyType keyType)
        {
            if (_keyLabelColors.TryGetValue(keyType, out LabelColor labelColor))
                return labelColor.Color;

            return _deffaultColor.Color;
        }
    }

    [Serializable]
    public struct LabelColor
    {
        [field: SerializeField, ColorUsage(true)] public Color Color { get; private set; }
    }
}