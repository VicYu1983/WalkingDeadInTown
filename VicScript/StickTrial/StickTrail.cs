using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace vic_game_lib{
	public class StickTrail : MonoBehaviour {

		public GameObject anchor_a;
		public GameObject anchor_b;

		public int level = 3;
		public float easing = .2f;

		Vector3[] verts;
		Mesh mesh;

		// Use this for initialization
		void Start () {
			
			mesh = GetComponent<MeshFilter>().mesh;
			mesh.Clear();

			SetVertices();
			SetTriangles();
		}

		void Update () {
			verts[0] = anchor_a.transform.position;
			verts[1] = anchor_b.transform.position;

			for( int i = 0; i < verts.Length - 2; i += 2 ){
				Vector3 p_0 = verts[i];
				Vector3 p_1 = verts[i+1];
				Vector3 p_2 = verts[i+2];
				Vector3 p_3 = verts[i+3];

				verts[i+2] = EasingTo(p_0, p_2);
				verts[i+3] = EasingTo(p_1, p_3);
			}
				
			mesh.vertices = verts;
		}

		void SetVertices(){
			List<Vector3> vs = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			for( int i = 0; i < level; ++i ){
				Vector3 v1 = new Vector3( i * .1f, 0, 0 );
				Vector3 v2 = new Vector3( i * .1f, 1, 0 );
				vs.Add(v1);
				vs.Add(v2);

				float uv_x = (float)i/ (float)(level-1);
				if( uv_x == 0 )
					uv_x = .05f;
				else if(uv_x == 1 ){
					uv_x = .95f;
				}

				uvs.Add( new Vector2( uv_x, 0 ));
				uvs.Add( new Vector2( uv_x, 1 ));
			}

			verts = new Vector3[vs.Count];
			for( int i = 0; i < vs.Count; ++i ) verts[i] = vs[i];
			mesh.vertices = verts;

			Vector2[] usedUv = new Vector2[uvs.Count];
			for( int i = 0; i < uvs.Count; ++i ) {
				usedUv[i] = uvs[i];
				print(uvs[i]);
			}
			mesh.uv = usedUv;
		}

		void SetTriangles(){
			List<int> list_tri = new List<int>();
			for( int i = 0; i < verts.Length - 2; i+=2 ){
				list_tri.Add( i );
				list_tri.Add( i+1 );
				list_tri.Add( i+3 );

				list_tri.Add( i );
				list_tri.Add( i+3 );
				list_tri.Add( i+2 );
			}

			int[] tris = new int[list_tri.Count];
			for( int i = 0; i < list_tri.Count; ++i ) tris[i] = list_tri[i];
			mesh.triangles = tris;
		}

		Vector3 EasingTo( Vector3 target, Vector3 child ){
			return child + ( target - child ) * easing;
		}
	}
}
