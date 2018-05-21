using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MapLayout.PosInfoDict))]
public class PosInfoDictDrawer : SerializableDictionaryPropertyDrawer {}
