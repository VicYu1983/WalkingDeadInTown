using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace vic_game_lib{
	public class EventDispatcher {
		private Dictionary<string,List<Action<object>>> methods = new Dictionary<string, List<Action<object>>>();

		private static EventDispatcher instance = null;

		private EventDispatcher(){

		}

		public static EventDispatcher GetInstance(){
			if(instance == null )	instance = new EventDispatcher();
			return instance;
		}

		public void AddListener( string name,  Action<object> func ){
			if( !methods.ContainsKey( name )){
				methods.Add( name, new List<Action<object>>() );
			}
			if( !methods[name].Contains( func ))
				methods[name].Add( func );
		}

		public void RemoveListener( string name, Action<object> func ){
			if( methods.ContainsKey( name )){
				foreach( Action<object> method in methods[name] ){
					if( method == func ){
						methods[name].Remove( method );
						break;
					}
				}
			}
		}

		public void DispatchEvent( string name, object args = null ){
			if( methods.ContainsKey( name )){
				List<Action<object>> ms =  methods[name];
				foreach( Action<object> method in ms ){
					method( args );
				}
			}
		}
	}
}
