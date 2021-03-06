using UnityEngine;
using System.Collections;
using System;

namespace Frontiers.World.WIScripts
{
		public class Key : WIScript
		{
				public override void OnInitialized()
				{
						worlditem.Props.Name.DisplayName = State.KeyName;
				}

				public KeyState State = new KeyState();
		}

		[Serializable]
		public class KeyState
		{
				public string KeyType = "SimpleKey";
				public string KeyTag = "Master";
				public string KeyName = "Key";
		}
}
