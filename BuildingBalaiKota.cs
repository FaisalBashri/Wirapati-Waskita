using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingBalaiKota : Node3D
{
	[Export] public int MaxHP = 1500;
	private int currentHP;
	private Label3D hpLabel;
	
	// Attack logic
	[Export] public float AttackRange = 4.0f;
	[Export] public int AttackDamage = 400;
	[Export] public float AttackCooldown = 2.0f;
	
	private float attackTimer = 0f;
	private bool isCharging = false;
	private bool isAttacking = false;
	
	private MeshInstance3D attackProgressMesh;
	private Area3D attackArea;
	private List<Node3D> targetsInRange = new List<Node3D>();
	
	public override void _Ready()
	{
		currentHP = MaxHP;
		hpLabel = GetNode<Label3D>("HPLabel");
		UpdateHPLabel();
	
		// Ambil node AttackProgress dan AttackRange
		attackProgressMesh = GetNode<MeshInstance3D>("AttackProgress");
		attackArea = GetNode<Area3D>("AttackRange");
	
		// Set shape radius sesuai AttackRange
		var shape = attackArea.GetNode<CollisionShape3D>("CollisionShape3D");
		if (shape.Shape is SphereShape3D sphere)
			sphere.Radius = AttackRange;
	
		// Event Area3D
		attackArea.BodyEntered += OnBodyEntered;
		attackArea.BodyExited += OnBodyExited;

		// Reset skala AttackProgress
		attackProgressMesh.Scale = new Vector3(1, 1, 1);
	}
	
	public void TakeDamage(int amount)
	{
		currentHP -= amount;
		currentHP = Math.Max(currentHP, 0);
		GD.Print($"Bangunan terkena damage {amount}. HP sekarang: {currentHP}");
	
		UpdateHPLabel();
	
		if (currentHP <= 0)
		{
			Destroy();
		}
	}
	
	private void OnBodyEntered(Node body)
	{
		// Cek layer 2
		if (body is CharacterBody3D character && ((1 << 1) & character.CollisionLayer) != 0)
		{
			if (!targetsInRange.Contains(character))
				targetsInRange.Add(character);
		}
	}
	
	private void OnBodyExited(Node body)
	{
		if (body is CharacterBody3D character)
			targetsInRange.Remove(character);
	}
	
	

	public override void _Process(double delta)
	{
		// Jika sedang tidak menyerang dan ada target, mulai serangan
		if (!isAttacking && targetsInRange.Count > 0)
		{
			isAttacking = true;
			attackTimer = 0f;
		}

		// Jika sedang menyerang, lanjutkan progress hingga selesai
		if (isAttacking)
		{
			attackTimer += (float)delta;
			float t = Mathf.Clamp(attackTimer / AttackCooldown, 0, 1);
			float scale = Mathf.Lerp(1f, 2.15f, t);
			attackProgressMesh.Scale = new Vector3(scale, 1, scale);

			if (attackTimer >= AttackCooldown)
			{
				// Damage hanya diberikan ke target yang masih valid dan masih ada di scene
				foreach (var target in targetsInRange)
				{
					if (IsInstanceValid(target) && target is ProtoController pc)
						pc.TakeDamage(AttackDamage);
				}
				// Reset progress dan status serangan
				attackTimer = 0f;
				attackProgressMesh.Scale = new Vector3(1, 1, 1);
				isAttacking = false;
			}
		}
		else
		{
			// Tidak sedang menyerang, pastikan progress bar normal
			attackProgressMesh.Scale = new Vector3(1, 1, 1);
		}
	}
	
	private void UpdateHPLabel()
	{
		if (hpLabel != null)
			hpLabel.Text = $"HP: {currentHP}/{MaxHP}";
	}

	private void Destroy()
	{
		GD.Print("Bangunan hancur!");
		QueueFree();
	}
}
