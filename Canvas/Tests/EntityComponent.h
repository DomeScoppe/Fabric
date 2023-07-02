#pragma once

#include "Test.h"
#include "TestScript.h"

#include "EngineAPI/Components.h"
#include "EngineAPI/GameEntity.h"
#include "EngineAPI/Scene.h"

#include <iostream>
#include <ctime>

using namespace fabric;

struct Random
{
	u32 random;
};

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
		basic_creation_test();
	}

	virtual void shutdown() override 
	{  

	}

private:
	void basic_creation_test()
	{
		Random r;
		r.random = rand() % 20;
		entity::entity e1 = scene::create_entity();
		id::id_type e1_id = id::index(e1.get_id());
		id::id_type e1_generation = id::generation(e1.get_id());

		e1.add_component<Random>(r);
		Random& r_test = e1.get_component<Random>();
		r_test.random = r.random * 2;

		r = e1.get_component<Random>();

		scene::remove_entity(e1);
		entity::entity e2 = scene::create_entity();
		id::id_type e2_id = id::index(e2.get_id());
		id::id_type e2_generation = id::generation(e2.get_id());

		entity::entity e3 = scene::create_entity();
		id::id_type e3_id = id::index(e3.get_id());
		id::id_type e3_generation = id::generation(e3.get_id());

		e2.add_component<Random>({ 5 });
		e2.add_component<TestScript>({ e2 });
		e3.add_component<Random>({ 7 });
		e3.add_component<TestScript>({ e3 });

		entity::entity e4 = scene::create_entity();
		e4.add_component<Random>();
		e4.add_component<TestScript>({ e4 });

		const auto& all_random_components = scene::get_all<Random>();

		// Test for data components
		for (auto& component : all_random_components)
		{
			component.random *= 10;
		}

		const auto& all_script_components = scene::get_all<TestScript>();

		// Test for script components
		for (auto& component : all_script_components)
		{
			component.create();
		}

		const auto& all_transform_components = scene::get_all<component::transform>();

		// Test for components added via script
		for (auto& component : all_transform_components)
		{
			std::cout << "Position: [x: " << component.position.x << ", y: " << component.position.y << ", z: " << component.position.z << "]" << std::endl;
		}

		Random& r1 = e1.get_component<Random>();
		Random& r2 = e2.get_component<Random>();
		Random& r3 = e3.get_component<Random>();
		Random& r4 = e4.get_component<Random>();

		scene::remove_entity(e2);
		scene::remove_entity(e3);
		scene::remove_entity(e4);
	}

private:
	u64 _count = 0;
};