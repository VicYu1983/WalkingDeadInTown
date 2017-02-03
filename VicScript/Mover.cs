using UnityEngine;
using System.Collections;

namespace vic_game_lib{
	public class Mover : MonoBehaviour {

		public Vector3 acc = new Vector3();
		public Vector3 vel = new Vector3();
		public Vector3 align = new Vector3(0,1,0);

		public float mass = 1;
		public float friction = 0;
		public bool autoRotate = false;

		public Vector3 GetPos(){
			return transform.localPosition;
		}

		public void SetPos( Vector3 pos ){
			transform.localPosition = pos;
		}

		public void ApplyForce( Vector3 force ){
			acc += force / mass;
		}
		
		private void RotateAlongSpeed(){
			Vector3 spin_vec = Vector3.Cross( align, vel );
			float angle = Vector3.Angle( align, vel );
			Quaternion rot = Quaternion.AngleAxis( angle, vel );
			this.transform.localRotation = rot;
		}

		void Start(){
			align.Normalize();
		}

		void Update () {
			if( friction != 0 ){
				ApplyForce( -vel * friction );
			}

			vel += acc;
			if( Mathf.Abs( vel.magnitude ) < .001f ){
				vel.x = vel.y = vel.z = 0;
			}

			SetPos( GetPos() + vel );

			acc.x = acc.y = acc.z = 0;

			if( autoRotate && vel.magnitude != 0 )
				RotateAlongSpeed();
		}
	}
}
