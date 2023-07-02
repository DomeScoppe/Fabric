#include "TestScript.h"

#include <iostream>

REGISTER_SCRIPT(TestScript);

void TestScript::create()
{
	entity::entity entity = get_entity();
	entity.add_component<component::transform>({ {(float)entity.get_id(),0.0f,0.0f},{0.0f,0.0f,0.0f},{1.0f,1.0f,1.0f}});
	std::cout << "Added script to entity " << entity.get_id() << std::endl;
}

void TestScript::update(float dt)
{
	entity::entity entity = get_entity();
	auto& transform = entity.get_component<component::transform>();
	transform.position = { transform.position.x * dt, transform.position.y * dt, transform.position.z * dt };
}

void TestScript::destroy()
{
}
