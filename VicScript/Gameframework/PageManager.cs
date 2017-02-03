using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace vic_game_lib{
	public class PageManager {

		private GameObject container;
		private List<GameObject> prefabs;
		private GameObject currentPage;

		private static PageManager instance;

		private PageManager(){

		}

		public void SetContainer( GameObject container ){
			this.container = container;
		}

		public void AddPrefab( GameObject prefab ){
			prefabs.Add( prefab );
		}

		public void SetPrefabs( List<GameObject> prefabs ){
			this.prefabs = prefabs;
		}

		public static PageManager GetInstance(){
			if( instance == null ) instance = new PageManager();
			return instance;
		}
		
		public void ChangeLanguage( Language lang ){
			if( currentPage != null ){
				currentPage.GetComponent<Page>().SetLanguage( lang );
			}
		}

		GameObject GetPagePrefabByName( string pageName ){
			foreach( GameObject page in prefabs ){
				if(page.name == pageName ) return page;
			}
			return null;
		}

		public void ChangePage( string pageName, object args = null ){
			if( this.container == null ){
				throw new Exception( "should set container first!" );
			}
			if( currentPage != null && currentPage.name == pageName ) return;
			if( currentPage != null ){
				currentPage.GetComponent<Page>().Close();
				GameObject.Destroy( currentPage );
			}

			GameObject targetPagePrefab = GetPagePrefabByName( pageName );
			if( targetPagePrefab == null ){
				throw new Exception( "prefab " + pageName + " not found!" );
			}
			currentPage = GameObject.Instantiate( targetPagePrefab );
			currentPage.transform.parent = container.transform;
			currentPage.SetActive( true );
			currentPage.GetComponent<Page>().Open( args );
		}
	}
}
