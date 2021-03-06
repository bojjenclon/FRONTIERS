using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Frontiers;
using Frontiers.World;
using Frontiers.World.WIScripts;

namespace Frontiers.World {
	public class ExplosionEffectSphere : EffectSphere
	{
			public DamagePackage ExplosionDamage;
			public float ForceAtCenter = 1f;
			public float ForceAtEdge = 0f;
			public float MinimumForce = 0f;

			public override void Start()
			{
					base.Start();

					OnIntersectItemOfInterest += SendDamageToItem;
					OcclusionLayerMask = Globals.LayerSolidTerrain | Globals.LayerObstacleTerrain | Globals.LayerStructureTerrain;
			}

			public void SendDamageToItem()
			{
					while (ItemsOfInterest.Count > 0) {
							IItemOfInterest itemOfInterest = ItemsOfInterest.Dequeue();
							DamagePackage packageCopy = ObjectClone.Clone <DamagePackage>(ExplosionDamage);
							float adjustedDamage = Mathf.Lerp(ExplosionDamage.DamageSent * ForceAtEdge, ExplosionDamage.DamageSent * ForceAtCenter, NormalizedRadius);
							packageCopy.DamageSent = adjustedDamage;
							packageCopy.ForceSent = Mathf.Max(MinimumForce, packageCopy.ForceSent * NormalizedRadius);
							packageCopy.Point = itemOfInterest.Position;
							packageCopy.Origin = transform.position;
							packageCopy.Target = itemOfInterest;
								//Debug.Log("Force: " + packageCopy.Force.ToString());
							DamageManager.Get.SendDamage(packageCopy);
							//characters and creatures are automatically stunned
							if (itemOfInterest.IOIType == ItemOfInterestType.WorldItem) {
									if (itemOfInterest.worlditem.Is <Creature>(out mCreatureCheck)) {
											mCreatureCheck.TryToStun(adjustedDamage);
									} else if (itemOfInterest.worlditem.Is <Character>(out mCharacterCheck)) {
											mCharacterCheck.TryToStun(adjustedDamage);
									}
							}
					}
			}

			protected static Character mCharacterCheck;
			protected static Creature mCreatureCheck;
	}
}
