using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace vic_game_lib{
	abstract public class Page : MonoBehaviour {

		protected Button[] btns;

		void Awake(){
			btns = GetComponentsInChildren<Button>();
		}

		void Start(){
			
		}

		abstract public void Open( object args = null );
		abstract public void Close( object args = null );
		abstract protected void ToEn();
		abstract protected void ToTw();

		protected Button GetButtonByName( string name ){
			foreach( Button btn in btns ){
				if( btn.name == name )
					return btn;
			}
			return null;
		}

		protected void SetButtonAction( Action<Button> func ){
			foreach( Button btn in btns ){
				Button target = btn;
				btn.onClick.AddListener( () => {
					func( target );
				});
			}
		}

		protected void ClearButtonAction(){
			foreach( Button btn in btns ){
				btn.onClick.RemoveAllListeners();
			}
		}

		public void SetLanguage( Language lang ){
			switch( lang ){
			case Language.EN:
				ToEn();
				break;
			case Language.TW:
				ToTw();
				break;
			}
		}
	}
}
