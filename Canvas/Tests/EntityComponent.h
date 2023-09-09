#pragma once

#include "Test.h"

#include "Fabric.h"

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
		entity_test();
		//component_test();
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

		entity::entity e1 = scene::create_entity();

		e1.add_component<Random>();

		if(e1.has_component<Random>())
			e1.add_component<Random>(r);

		scene::remove_entity(e1);
	}

	void relationship_test()
	{

	}

	void script_test()
	{

	}

private:
	u64 _count = 0;
};