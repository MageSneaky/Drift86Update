using System;
using UnityEngine;

public class Controls : MonoBehaviour
{
	private void Start()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
		if (this.currentColor == null)
		{
			this.currentColor = "Red";
		}
	}

	private void Update()
	{
		this.steerBall();
		this.changeColor();
	}

	public void changeColor()
	{
		if (Input.GetButtonDown(this.playerNumber + "_ZRed"))
		{
			base.GetComponent<MeshRenderer>().material = this.redMaterial;
			this.currentColor = "Red";
		}
		if (Input.GetButtonDown(this.playerNumber + "_ZBlue"))
		{
			base.GetComponent<MeshRenderer>().material = this.blueMaterial;
			this.currentColor = "Blue";
		}
		if (Input.GetButtonDown(this.playerNumber + "_ZGreen"))
		{
			base.GetComponent<MeshRenderer>().material = this.greenMaterial;
			this.currentColor = "Green";
		}
		if (Input.GetButtonDown(this.playerNumber + "_ZYellow"))
		{
			base.GetComponent<MeshRenderer>().material = this.yellowMaterial;
			this.currentColor = "Yellow";
		}
		if (Input.GetButtonDown(this.playerNumber + "_ZPrevious"))
		{
			this.cycleColor(false, this.currentColor);
		}
		if (Input.GetButtonDown(this.playerNumber + "_ZNext"))
		{
			this.cycleColor(true, this.currentColor);
		}
		if (Input.GetAxis(this.playerNumber + "_ZCycle") < 0f && !this.cycleLock)
		{
			this.cycleColor(false, this.currentColor);
			this.cycleLock = true;
			base.Invoke("unlock", 100f / (Input.GetAxis(this.playerNumber + "_ZCycle") * 2000f));
		}
		if (Input.GetAxis(this.playerNumber + "_ZCycle") > 0f && !this.cycleLock)
		{
			this.cycleColor(true, this.currentColor);
			this.cycleLock = true;
			base.Invoke("unlock", 100f / (Input.GetAxis(this.playerNumber + "_ZCycle") * 2000f));
		}
	}

	public void steerBall()
	{
		this.horizontal = Input.GetAxis(this.playerNumber + "_Horizontal");
		this.vertical = Input.GetAxis(this.playerNumber + "_Vertical");
		this.horizontal *= -this.sensitivity;
		this.vertical *= -this.sensitivity;
		this.actionVectorPosition.x = this.horizontal;
		this.actionVectorPosition.y = 0f;
		this.actionVectorPosition.z = this.vertical;
		this.myRigidbody.AddForce(this.actionVectorPosition);
	}

	public void cycleColor(bool forward, string current)
	{
		if (forward)
		{
			if (current.Equals("Red"))
			{
				base.GetComponent<MeshRenderer>().material = this.greenMaterial;
				this.currentColor = "Green";
				return;
			}
			if (current.Equals("Green"))
			{
				base.GetComponent<MeshRenderer>().material = this.blueMaterial;
				this.currentColor = "Blue";
				return;
			}
			if (current.Equals("Blue"))
			{
				base.GetComponent<MeshRenderer>().material = this.yellowMaterial;
				this.currentColor = "Yellow";
				return;
			}
			base.GetComponent<MeshRenderer>().material = this.redMaterial;
			this.currentColor = "Red";
			return;
		}
		else
		{
			if (current.Equals("Red"))
			{
				base.GetComponent<MeshRenderer>().material = this.yellowMaterial;
				this.currentColor = "Yellow";
				return;
			}
			if (current.Equals("Green"))
			{
				base.GetComponent<MeshRenderer>().material = this.redMaterial;
				this.currentColor = "Red";
				return;
			}
			if (current.Equals("Blue"))
			{
				base.GetComponent<MeshRenderer>().material = this.greenMaterial;
				this.currentColor = "Green";
				return;
			}
			base.GetComponent<MeshRenderer>().material = this.blueMaterial;
			this.currentColor = "Blue";
			return;
		}
	}

	public void unlock()
	{
		this.cycleLock = false;
	}

	public Material redMaterial;

	public Material greenMaterial;

	public Material blueMaterial;

	public Material yellowMaterial;

	public string playerNumber;

	private Rigidbody myRigidbody;

	private float horizontal;

	private float vertical;

	private float sensitivity = 5f;

	private Vector3 actionVectorPosition;

	private Vector3 computerVector;

	private string currentColor;

	private bool cycleLock;
}
