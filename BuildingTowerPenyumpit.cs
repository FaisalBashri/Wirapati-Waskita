using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingTowerPenyumpit : Node3D
{
	[Export] public int MaxHP = 300;
	[Export] public int AttackDamage = 10;
	[Export] public float AttackSpeed = 25.0f; // Kecepatan proyektil
	[Export] public PackedScene ProjectileScene;
	[Export] public float Cooldown = 1.0f; // Bisa diatur sesuai kebutuhan

	private int currentHP;
	private Label3D hpLabel;
	private Node3D AttackSource;
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

		AttackSource = GetNode<Node3D>("AttackSource");
	}

	private void OnBodyEntered(Node body)
	{
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
		if (!isAttacking && targetsInRange.Count > 0)
		{
			isAttacking = true;
			attackTimer = 0f;
		}

		if (isAttacking)
		{
			attackTimer += (float)delta;

			if (attackTimer >= Cooldown)
			{
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
		if (ProjectileScene == null || AttackSource == null || target == null)
			return;

		var projectile = ProjectileScene.Instantiate<Node3D>();
		GetParent().AddChild(projectile);
		projectile.GlobalTransform = AttackSource.GlobalTransform;

		Vector3 direction = (target.GlobalTransform.Origin - AttackSource.GlobalTransform.Origin).Normalized();

		if (projectile is RigidBody3D rigid)
		{
			rigid.LinearVelocity = direction * AttackSpeed;
			// Kirim damage ke proyektil jika perlu
			if (projectile.HasMethod("SetDamage"))
				projectile.Call("SetDamage", AttackDamage);
		}
		// Jika proyektil bukan RigidBody3D, tambahkan script gerak pada scene proyektil
	}

	public void TakeDamage(int amount)
	{
		currentHP -= amount;
		currentHP = Math.Max(currentHP, 0);
		GD.Print($"Tower Penyumpit terkena damage {amount}. HP sekarang: {currentHP}");

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
		GD.Print("Tower Penyumpit hancur!");
		QueueFree();
	}
}
