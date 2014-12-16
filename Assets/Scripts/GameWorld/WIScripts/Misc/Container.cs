using UnityEngine;
using System.Collections;
using Frontiers;
using Frontiers.World.Gameplay;
using System;
using Frontiers.GUI;
using System.Collections.Generic;

namespace Frontiers.World
{
		public class Container : WIScript
		{
				public string OpenText = "Open";//changed by whatever uses it
				public bool CanOpen = true;//whether it can be opened at all
				public bool CanUseToOpen = true;//whether OnPlayerUse opens it automatically

				public override bool CanBeCarried {
						get {
								return State.Type != ContainerType.ShopGoods;
						}
				}

				public override bool CanEnterInventory {
						get {
								return State.Type != ContainerType.ShopGoods;
						}
				}

				public override bool UnloadWhenStacked {
						get {
								return false;
						}
				}

				public Action OnOpenContainer;

				public override void OnStartup()
				{
						if (!worlditem.IsStackContainer) {
								worlditem.StackContainer = Stacks.Create.StackContainer(worlditem, worlditem.Group);
						} else {
								worlditem.StackContainer.Owner = worlditem;
						}
				}

				public ContainerState State = new ContainerState();

				public override void OnInitialized()
				{
						worlditem.OnPlayerUse += OnPlayerUse;
				}

				public void OnPlayerUse()
				{
						if (!CanOpen || !CanUseToOpen) {
								return;
						}
						//if we can't enter inventory
						//open the container
						OnOpenContainer.SafeInvoke();
						PrimaryInterface.MaximizeInterface("Inventory", "OpenStackContainer", worlditem.gameObject);
				}

				public override void PopulateOptionsList(System.Collections.Generic.List <GUIListOption> options, List <string> message)
				{
						if (CanOpen) {
								options.Add(new GUIListOption(OpenText, "Open"));
						}
				}

				public virtual void OnPlayerUseWorldItemSecondary(object secondaryResult)
				{
						OptionsListDialogResult dialogResult = secondaryResult as OptionsListDialogResult;

						switch (dialogResult.SecondaryResult) {
								case "Open":
										OnOpenContainer.SafeInvoke();
										PrimaryInterface.MaximizeInterface("Inventory", "OpenStackContainer", worlditem.gameObject);
										if (State.ReputationChangeOnOpen != 0) {
												Profile.Get.CurrentGame.Character.Rep.ChangeGlobalReputation(State.ReputationChangeOnOpen);
										}
										break;

								default:
										break;
						}
				}

				public static GenericWorldItem DefaultContainerGenericWorldItem {
						get {
								if (gDefaultContainerGenericWorldItem == null) {
										gDefaultContainerGenericWorldItem = new GenericWorldItem();
										gDefaultContainerGenericWorldItem.PackName = "Containers";
										gDefaultContainerGenericWorldItem.PrefabName = "Sack 1";
								}
								return gDefaultContainerGenericWorldItem;
						}
				}

				protected static GenericWorldItem gDefaultContainerGenericWorldItem;
		}

		[Serializable]
		public class ContainerState
		{
				public ContainerType Type = ContainerType.PersonalEffects;
				public int ReputationChangeOnOpen = 0;//for coffins mainly
		}

		public enum ContainerType
		{
				//TODO find a better way to restrict options
				PersonalEffects,
				ShopGoods,
		}
}