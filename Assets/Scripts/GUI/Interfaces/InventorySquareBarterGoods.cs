using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Frontiers;
using Frontiers.World;
using Frontiers.World.Gameplay;

namespace Frontiers.GUI
{
		public class InventorySquareBarterGoods : InventorySquareBarter
		{
				public override bool IsEnabled {
						get {
								return HasSession && Session.IsActive && base.IsEnabled;
						}
				}

				public override void SetProperties()
				{
						base.SetProperties();
						//MouseoverIcon = something something barter
						//TODO implemenet custom cursors
				}

				public override void Awake()
				{
						base.Awake();
						QuestItemHighlight.transform.localPosition = new Vector3(0f, 0f, -75f);
				}

				public int NumItems {
						get {
								int numItems = 0;
								if (HasStack) {
										numItems = mStack.NumItems - mNumGoodsSent;
								}
								return numItems;
						}
				}

				public bool SoldOut {
						get {
								if (HasStack && mStack.NumItems > 0) {
										return (mStack.NumItems - mNumGoodsSent) <= 0;
								}
								return false;
						}
				}

				public override void OnHover(bool isOver)
				{
						base.OnHover(isOver);
						if (isOver && HasSession) {
								if (NumItems > 0) {
										if (Stack.TopItem.IsWorldItem) {
												Session.InfoDisplay.PostInfo(gameObject, Examine.GetExamineInfo(Stack.TopItem.worlditem));
										} else {
												Session.InfoDisplay.PostInfo(gameObject, Examine.GetExamineInfo(Stack.TopItem.GetStackItem(WIMode.Stacked)));
										}
								}
						}
				}

				public override void OnClickSquare()
				{
						if (HasStack && mStack.TopItem.IsQuestItem) {
								return;
						}

						bool showMenu = false;
						//right-clicking will show a menu
						if (InterfaceActionManager.LastMouseClick == 1) {
								showMenu = true;
						}

						if (showMenu) {
								if (mStack.HasTopItem) {
										WorldItem topItem = null;
										WorldItemUsable usable = null;
										if (Stacks.Convert.TopItemToWorldItem(mStack, out topItem)) {
												if (mUsable != null) {
														mUsable.Finish();
														mUsable = null;
												}
												usable = topItem.gameObject.GetOrAdd <WorldItemUsable>();
												usable.IncludeInteract = false;
												usable.ShowDoppleganger = false;
												usable.TryToSpawn(true, out mUsable, NGUICamera);
												usable.ScreenTarget = transform;
												usable.ScreenTargetCamera = NGUICamera;
												usable.RequirePlayerFocus = false;
												usable.PauseWhileOpen = true;
												//usable end result *should* affect the new item
												return;
										}
								}
						}
						//by now our mouseover icon should be indicating
						//that we're sending this to the session goods
						if (NumItems > 0) {
								if (mStack.TopItem.IsQuestItem) {
										MasterAudio.PlaySound(MasterAudio.SoundType.PlayerInterface, "ButtonClickDisabled");
										return;
								}
								SendGoodsToSession(1);
								MasterAudio.PlaySound(SoundType, "InventoryPlaceStack");
						} else {
								MasterAudio.PlaySound(SoundType, SoundNameFailure);
						}
				}

				public override void OnDrag()
				{
						return;
				}

				public override void OnDrop()
				{
						return;
				}

				protected override void OnRefresh()
				{
						mNumGoodsSent = 0;
						if (HasSession && HasStack) {
								//get how many goods we've sent
								mNumGoodsSent = Session.NumGoodsSent(Party, mStack);
						}
						base.OnRefresh();
				}

				public override void SetInventoryStackNumber()
				{
						string stackNumberLabelText = string.Empty;
						int numItems = NumItems;
						bool soldOut = SoldOut;
						if (numItems > 0 && Stack.TopItem.IsQuestItem) {
								stackNumberLabelText = "(Mission item)";
						} else if (soldOut) {
								if (Party == BarterParty.Character) {
										stackNumberLabelText = "(Sold Out)";
								} else {
										stackNumberLabelText = "(None Left)";
								}
						} else if (numItems >= 1) {
								stackNumberLabelText = numItems.ToString();
						}
						StackNumberLabel.text = stackNumberLabelText;
						StackNumberLabel.transform.localScale = Vector3.one * 18f;
				}

				public void SendGoodsToSession(int numGoodsToSend)
				{
						mNumGoodsSent += numGoodsToSend;
						Session.AddGoods(mStack, numGoodsToSend, Party);
						//this will automatically cause a refresh
				}

				protected int mNumGoodsSent = 0;
		}
}