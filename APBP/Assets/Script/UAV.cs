using System.Collections.Generic;
using UnityEngine;

public class UAV : MonoBehaviour
{
	public Transform goal;
	public float epsilon = 1f;
	public float zeta = 1f;

	private Transform goal_standard_points;
	private Transform uav;
	private Transform uav_standard_points;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		goal_standard_points = goal.GetChild(0);
		uav = this.transform;
		uav_standard_points = uav.GetChild(0);

		epsilon = 5f;
		zeta = 0.1f;
	}

    // Update is called once per frame
    void Update() { }

	private void FixedUpdate()
	{
		List<float> coor = F_Q();
		Vector3 dir = new Vector3(coor[0], coor[1], coor[2]);
		Vector3 deg = new Vector3(coor[3], coor[4], coor[5]);
		uav.position = uav.position + dir * Time.fixedDeltaTime;
		uav.eulerAngles = uav.eulerAngles + deg;
	}

	private List<float> F_Q()
	{
		List<float> f_q = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f };

		List<List<List<float>>> f_att = F_att();
		List<List<List<float>>> jacobian = J();
		List<List<float>> total_F_att;

		total_F_att = MatMul(T(jacobian[0]), f_att[0]);
		for (int i = 1; i < uav_standard_points.childCount; i++)
		{
			List<List<float>> JxF_w = MatMul(T(jacobian[i]), f_att[i]);
			total_F_att = MatAdd(JxF_w, total_F_att);
		}

		for (int i = 0; i < total_F_att.Count; i++) {
			f_q[i] = total_F_att[i][0];
		}

		return f_q;
	}

	private List<List<float>> MatAdd(List<List<float>> matA, List<List<float>> matB)
	{
		List<List<float>> result = new List<List<float>>();

		for (int i = 0; i < matA.Count; i++)
		{
			List<float> row = new List<float>();

			for (int j = 0; j < matA[i].Count; j++)
			{
				row.Add(matA[i][j] + matB[i][j]);
			}
			result.Add(row);
		}

		return result;
	}

	private List<List<float>> MatMul(List<List<float>> matA, List<List<float>> matB)
	{
		List<List<float>> result = new List<List<float>>();

		for (int i = 0; i < matA.Count; i++)
		{
			List<float> row = new List<float>();
			for (int k = 0; k < matB[0].Count; k++)
			{
				float sum = 0f;

				for (int j = 0; j < matB.Count; j++)
				{
					sum += matA[i][j] * matB[j][k];
				}
				row.Add(sum);
			}
			result.Add(row);
		}

		return result;
	}

	private List<List<List<float>>> J()
	{
		float phi = uav.eulerAngles.x;
		float theta = uav.eulerAngles.y;
		float psi = uav.eulerAngles.z;

		List<List<List<float>>> jacobian = new List<List<List<float>>>();

		List<List<float>> j1 = new List<List<float>>();
		j1.Add(new List<float> {
			1f,
			0f,
			0f,
			0f,
			(69*Cos(theta))/50f + (67*Cos(psi)*Sin(theta))/100f,
			(67*Cos(theta)*Sin(psi))/100f
		});
		j1.Add(new List<float> {
			0f,
			1f,
			0f,
			(69*Cos(phi)*Cos(theta))/50f + (67*Sin(phi)*Sin(psi))/100f + (67*Cos(phi)*Cos(psi)*Sin(theta))/100f,
			(67*Cos(psi)*Cos(theta)*Sin(phi))/100f - (69*Sin(phi)*Sin(theta))/50f,
			- (67*Cos(phi)*Cos(psi))/100f - (67*Sin(phi)*Sin(psi)*Sin(theta))/100f
		});
		j1.Add(new List<float> {
			0f,
			0f,
			1f,
			(69*Cos(theta)*Sin(phi))/50f - (67*Cos(phi)*Sin(psi))/100f + (67*Cos(psi)*Sin(phi)*Sin(theta))/100f,
			(69*Cos(phi)*Sin(theta))/50f - (67*Cos(phi)*Cos(psi)*Cos(theta))/100f,
			(67*Cos(phi)*Sin(psi)*Sin(theta))/100f - (67*Cos(psi)*Sin(phi))/100f
		});
		jacobian.Add(j1);

		List<List<float>> j2 = new List<List<float>>();
		j2.Add(new List<float> {
			1f,
			0f,
			0f,
			0f,
			(67*Cos(psi)*Sin(theta))/100f - (69*Cos(theta))/50f,
			(67*Cos(theta)*Sin(psi))/100f
		});
		j2.Add(new List<float> {
			0f,
			1f,
			0f,
			(67*Sin(phi)*Sin(psi))/100f - (69*Cos(phi)*Cos(theta))/50f + (67*Cos(phi)*Cos(psi)*Sin(theta))/100f,
			(69*Sin(phi)*Sin(theta))/50f + (67*Cos(psi)*Cos(theta)*Sin(phi))/100f,
			- (67*Cos(phi)*Cos(psi))/100f - (67*Sin(phi)*Sin(psi)*Sin(theta))/100f
		});
		j2.Add(new List<float> {
			0f,
			0f,
			1f,
			(67*Cos(psi)*Sin(phi)*Sin(theta))/100f - (69*Cos(theta)*Sin(phi))/50f - (67*Cos(phi)*Sin(psi))/100f,
			- (69*Cos(phi)*Sin(theta))/50f - (67*Cos(phi)*Cos(psi)*Cos(theta))/100f,
			(67*Cos(phi)*Sin(psi)*Sin(theta))/100f - (67*Cos(psi)*Sin(phi))/100f
		});
		jacobian.Add(j2);

		List<List<float>> j3 = new List<List<float>>();
		j3.Add(new List<float> {
			1f,
			0f,
			0f,
			0f,
			-(71*Cos(psi)*Sin(theta))/100f,
			-(71*Cos(theta)*Sin(psi))/100f
		});
		j3.Add(new List<float> {
			0f,
			1f,
			0f,
			- (71*Sin(phi)*Sin(psi))/100f - (71*Cos(phi)*Cos(psi)*Sin(theta))/100f,
			-(71*Cos(psi)*Cos(theta)*Sin(phi))/100f,
			(71*Cos(phi)*Cos(psi))/100f + (71*Sin(phi)*Sin(psi)*Sin(theta))/100f
		});
		j3.Add(new List<float> {
			0f,
			0f,
			1f,
			(71*Cos(phi)*Sin(psi))/100f - (71*Cos(psi)*Sin(phi)*Sin(theta))/100f,
			(71*Cos(phi)*Cos(psi)*Cos(theta))/100f,
			(71*Cos(psi)*Sin(phi))/100f - (71*Cos(phi)*Sin(psi)*Sin(theta))/100f
		});
		jacobian.Add(j3);

		return jacobian;
	}

	private List<List<List<float>>> F_att()
	{
		Vector3 dist_vec;
		float dist;
		List<List<List<float>>> total_vec = new List<List<List<float>>>();

		for (int i = 0; i < uav_standard_points.childCount; i++)
		{
			List<List<float>> f_att_list = new List<List<float>>();

			dist_vec = uav_standard_points.GetChild(i).position - goal_standard_points.GetChild(i).position;
			dist = dist_vec.magnitude;
			Vector3 f_att_vec;

			if (dist > epsilon)
			{
				f_att_vec = -((epsilon * zeta * dist_vec) / dist);
				f_att_list.Add(new List<float> { f_att_vec.x });
				f_att_list.Add(new List<float> { f_att_vec.y });
				f_att_list.Add(new List<float> { f_att_vec.z });
			}
			else
			{
				f_att_vec = -(zeta * dist_vec);
				f_att_list.Add(new List<float> { f_att_vec.x });
				f_att_list.Add(new List<float> { f_att_vec.y });
				f_att_list.Add(new List<float> { f_att_vec.z });
			}
			total_vec.Add(f_att_list);
		}

		return total_vec;
	}

	private List<List<float>> T(List<List<float>> mat)
	{
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

	private float Sin(float deg)
	{
		return Mathf.Sin(deg * Mathf.Deg2Rad);
	}

	private float Cos(float deg)
	{
		return Mathf.Cos(deg * Mathf.Deg2Rad);
	}
}
