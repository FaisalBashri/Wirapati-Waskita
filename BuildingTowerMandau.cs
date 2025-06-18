using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingTowerMandau : Node3D
{
	[Export] public int MaxHP = 400;
	private int currentHP;
	private Label3D hpLabel;

	[Export] public PackedScene ProjectileScene;
	[Export] public float Cooldown = 1.5f;
	[Export] public float ProjectileSpeed = 15.0f;

	private Node3D pivotNode;
	private List<Node3D> targetsInRange = new List<Node3D>();
	private bool isAttacking = false;
	private float attackTimer = 0f;

	public override void _Ready()
	{
		currentHP = MaxHP;
		hpLabel = GetNodeOrNull<Label3D>("HPLabel");
		UpdateHPLabel();

		var attackRange = GetNode<Area3D>("AttackRange");
		attackRange.BodyEntered += OnBodyEntered;
		attackRange.BodyExited += OnBodyExited;

		pivotNode = GetNode<Node3D>("PivotNode");
	}

	private void OnBodyEntered(Node body)
	{
		// Deteksi hanya karakter (misal: ProtoController)
		if (body is CharacterBody3D character && !targetsInRange.Contains(character))
		{
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
		// Mulai serangan jika ada target dan belum menyerang
		if (!isAttacking && targetsInRange.Count > 0)
		{
			isAttacking = true;
			attackTimer = 0f;
		}

		// Jika sedang menyerang, proses cooldown dan tembak
		if (isAttacking)
		{
			attackTimer += (float)delta;

			if (attackTimer >= Cooldown)
			{
				// Tembak ke semua target yang masih valid
				foreach (var target in targetsInRange)
				{
					if (IsInstanceValid(target))
						ShootProjectile(target);
				}
				attackTimer = 0f;
				isAttacking = false;
			}
		}
	}

	private void ShootProjectile(Node3D target)
	{
		if (ProjectileScene == null || pivotNode == null || target == null)
			return;

		var projectile = ProjectileScene.Instantiate<Node3D>();
		GetParent().AddChild(projectile);
		projectile.GlobalTransform = pivotNode.GlobalTransform;

		Vector3 direction = (target.GlobalTransform.Origin - pivotNode.GlobalTransform.Origin).Normalized();

		if (projectile is RigidBody3D rigid)
		{
			rigid.LinearVelocity = direction * ProjectileSpeed;
		}
		// Jika Mandau.tscn bukan RigidBody3D, tambahkan script gerak pada Mandau.tscn
	}

		public void TakeDamage(int amount)
	{
		currentHP -= amount;
		currentHP = Math.Max(currentHP, 0);
		GD.Print($"Tower Mandau terkena damage {amount}. HP sekarang: {currentHP}");

		UpdateHPLabel();

		if (currentHP <= 0)
		{
			Destroy();
		}
	}

	private void UpdateHPLabel()
	{
		if (hpLabel != null)
			hpLabel.Text = $"HP: {currentHP}/{MaxHP}";
	}

	private void Destroy()
	{
		GD.Print("Tower Mandau hancur!");
		QueueFree();
	}
}
