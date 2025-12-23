using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class AutoCar : MonoBehaviour
{
	public Transform goal;
	public float epsilon = 1f;
	public float zeta = 1f;

	private Transform car;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		car = this.transform;

		epsilon = 0.85f;
		zeta = 0.5f;
	}

	// Update is called once per frame
	void Update() { }

	private void FixedUpdate()
	{
		List<float> coor = F_Q();
		Vector3 dir = new Vector3(coor[0], 0f, coor[1]);
		Vector3 deg = new Vector3(0f, coor[2] * Mathf.Rad2Deg, 0f);
		car.position = car.position + dir * Time.fixedDeltaTime;
		car.eulerAngles = car.eulerAngles + deg;
	}

	private List<float> F_Q() {
		List<float> f_q = new List<float> { 0f, 0f, 0f };

		List<List<List<float>>> f_att = F_att();
		List<List<List<float>>> jacobian = J();
		List<List<float>> total_F_att;

		total_F_att = MatMul(T(jacobian[0]), f_att[0]);
		for (int i = 1; i < car.childCount; i++) {
			total_F_att = MatAdd(MatMul(T(jacobian[i]), f_att[i]), total_F_att);
		}

		f_q[0] = total_F_att[0][0];
		f_q[1] = total_F_att[1][0];
		f_q[2] = total_F_att[2][0];

		return f_q;
	}

	private List<List<float>> MatAdd(List<List<float>> matA, List<List<float>> matB) {
		List<List<float>> result = new List<List<float>>();

		for (int i = 0; i < matA.Count; i++) {
			List<float> row = new List<float>();

			for (int j = 0; j < matA[i].Count; j++) {
				row.Add(matA[i][j] + matB[i][j]);
			}
			result.Add(row);
		}

		return result;
	}

	private List<List<float>> MatMul(List<List<float>> matA, List<List<float>> matB) {
		List<List<float>> result = new List<List<float>>();

		for (int i = 0; i < matA.Count; i++) {
			List<float> row = new List<float>();
			for (int k = 0; k < matB[0].Count; k++) {
				float sum = 0f;

				for (int j = 0; j < matB.Count; j++) {
					sum += matA[i][j] * matB[j][k];
				}
				row.Add(sum);
			}
			result.Add(row);
		}

		return result;
	}

	private List<List<List<float>>> J() {
		float a_x = 1.5f;
		float a_z = 1f;
		List<List<List<float>>> jacobian = new List<List<List<float>>>();

		List<List<float>> j1 = new List<List<float>>();
		j1.Add(new List<float> { 1f, 0f, 0f });
		j1.Add(new List<float> { 0f, 1f, 0f });
		jacobian.Add(j1);

		List<List<float>> j2 = new List<List<float>>();
		j2.Add(new List<float> { 1f, 0f, -a_x * (Sin(car.eulerAngles.y) * Mathf.Deg2Rad) });
		j2.Add(new List<float> { 0f, 1f, a_x * (Cos(car.eulerAngles.y) * Mathf.Deg2Rad) });
		jacobian.Add(j2);

		List<List<float>> j3 = new List<List<float>>();
		j3.Add(new List<float> { 1f, 0f, -a_z * (Cos(car.eulerAngles.y) * Mathf.Deg2Rad) - a_x * (Sin(car.eulerAngles.y) * Mathf.Deg2Rad) });
		j3.Add(new List<float> { 0f, 1f, a_x * (Cos(car.eulerAngles.y) * Mathf.Deg2Rad) - a_z * (Sin(car.eulerAngles.y) * Mathf.Deg2Rad) });
		jacobian.Add(j3);

		return jacobian;
	}

	private List<List<List<float>>> F_att()
	{
		Vector3 dist_vec;
		float dist;
		List<List<List<float>>> total_vec = new List<List<List<float>>>();

		for (int i = 0; i < car.childCount; i++) {
			List<List<float>> f_att_list = new List<List<float>>();

			dist_vec = car.GetChild(i).position - goal.GetChild(i).position;
			dist = dist_vec.magnitude;
			Vector3 f_att_vec;

			if (dist > epsilon)
			{
				f_att_vec = -((epsilon * zeta * dist_vec) / dist);
				f_att_list.Add(new List<float> { f_att_vec.x });
				f_att_list.Add(new List<float> { f_att_vec.z });
			} else {
				f_att_vec = -(zeta * dist_vec);
				f_att_list.Add(new List<float> { f_att_vec.x });
				f_att_list.Add(new List<float> { f_att_vec.z });
			}
			total_vec.Add(f_att_list);
		}

		return total_vec;
	}

	private List<List<float>> T(List<List<float>> mat) {
		List<List<float>> result = new List<List<float>>();

		for (int j = 0; j < mat[0].Count; j++)
		{
			List<float> tmp = new List<float>();

			for (int i = 0; i < mat.Count; i++)
			{
				tmp.Add(mat[i][j]);
			}
			result.Add(tmp);
		}

		return result;
	}

	private float Sin(float deg) {
		return Mathf.Sin( deg * Mathf.Deg2Rad );
	}

	private float Cos(float deg) {
		return Mathf.Cos( deg * Mathf.Deg2Rad );
	}
}
