#pragma once

#include "Test.h"

#include "Fabric.h"

#include <iostream>
#include <ctime>

using namespace fabric;

void system_func()
{
	std::cout << "System working!" << std::endl;
}

struct Random
{
	u32 random;
};

struct Transform
{
	float position[3];
};

struct Tag {};

class engine_test : public test
{
public:
	virtual bool initialize() override
	{
		srand((u32)time(nullptr));

		return true;
	}

	virtual void run() override
	{ 
		entity_test();
		component_test();
		system_test();

		scene::run_systems();
		std::cin.get();
	}

	virtual void shutdown() override
	{  

	}

private:
	void entity_test()
	{
		entity::entity e1 = scene::create_entity();
		entity::entity e2 = scene::create_entity();
		entity::entity e3 = scene::create_entity();
		entity::entity e4 = scene::create_entity();

		std::cout << "Entity id: " << e1.get_id() << std::endl;
		std::cout << "Entity id: " << e2.get_id() << std::endl;
		std::cout << "Entity id: " << e3.get_id() << std::endl;
		std::cout << "Entity id: " << e4.get_id() << std::endl;

		scene::remove_entity(e1);
		scene::remove_entity(e2);
		scene::remove_entity(e3);
		scene::remove_entity(e4);

		entity::entity e5 = scene::create_entity();
		entity::entity e6 = scene::create_entity();
		entity::entity e7 = scene::create_entity();
		entity::entity e8 = scene::create_entity();

		std::cout << "Entity id: " << e5.get_id() << std::endl;
		std::cout << "Entity id: " << e6.get_id() << std::endl;
		std::cout << "Entity id: " << e7.get_id() << std::endl;
		std::cout << "Entity id: " << e8.get_id() << std::endl;

		scene::remove_entity(e5);
		scene::remove_entity(e6);
		scene::remove_entity(e7);
		scene::remove_entity(e8);

		entity::entity e9 = scene::create_entity();
		entity::entity e10 = scene::create_entity();
		entity::entity e11 = scene::create_entity();
		entity::entity e12 = scene::create_entity();

		std::cout << "Entity id: " << e9.get_id() << std::endl;
		std::cout << "Entity id: " << e10.get_id() << std::endl;
		std::cout << "Entity id: " << e11.get_id() << std::endl;
		std::cout << "Entity id: " << e12.get_id() << std::endl;

		scene::remove_entity(e9);
		scene::remove_entity(e10);
		scene::remove_entity(e11);
		scene::remove_entity(e12);
	}

	void component_test()
	{
		Random r;
		r.random = rand() % 20;

		Transform transform;
		transform.position[0] = 1.0f;
		transform.position[1] = 2.0f;
		transform.position[2] = 3.0f;

		entity::entity e1 = scene::create_entity();

		e1.add_component<Tag>();

		e1.add_component<Random>();

		Random r1 = e1.get_component<Random>();

		e1.add_component<Transform>(transform);

		if (e1.has_component<Random>())
		{
			e1.add_component<Random>(r);
			
			std::cout << "Random: " << r1.random << std::endl;
		}

		transform.position[0] = 10.0f;
		transform.position[1] = 20.0f;
		transform.position[2] = 30.0f;

		if (e1.has_component<Transform>())
		{
			e1.add_component<Transform>(transform);

			Transform t = e1.get_component<Transform>();

			std::cout << "Position X: " << t.position[0] << std::endl;
			std::cout << "Position Y: " << t.position[1] << std::endl;
			std::cout << "Position Z: " << t.position[2] << std::endl;

		}

		e1.remove_component<Tag>();
		e1.remove_component<Random>();
		e1.remove_component<Transform>();

		scene::remove_entity(e1);
	}

	void system_test()
	{
		REGISTER_SYSTEM(Tag, system_func, Transform, Random);
	}

private:
	u64 _count = 0;
};