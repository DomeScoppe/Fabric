#pragma once

#include "Entity.h"
#include "../Utilities/TypeList.h"

namespace fabric::scene
{
	struct None {};

	template<typename Component>
	using Owner = typename utl::type_list<Component>;

	template<typename Head, typename... Tail>
	using Dependencies = typename utl::type_list<Head, Tail...>;

	template<typename Component, typename Head, typename... Tail>
	struct system
	{
		Owner<Component> owner;
		Dependencies<Head, Tail...> dependencies;
		void (*function)();
	};

	/// Convenience macro for registering systems.
	/// @param OWNER: Component that owns the system
	/// @param FN: Callback to system behavior function
	/// @param ...: Dependencies that must be resolved before this system can run
#define REGISTER_SYSTEM(OWNER, FN, ...)							\
		scene::Owner<OWNER> OWNER##_ownership;					\
		scene::Dependencies<__VA_ARGS__> OWNER##_dependencies;	\
		scene::system<OWNER, __VA_ARGS__> OWNER##_system		\
		{														\
			.owner = OWNER##_ownership,							\
			.dependencies = OWNER##_dependencies,				\
			.function = &FN										\
		};														\
		scene::register_system(OWNER##_system);
}

namespace fabric::ecs
{
	TYPED_ID(system_id);

	entity_id create_entity();
	void remove_entity(entity_id id);
	bool is_alive(entity_id id);

	system_id register_system(component_id owner, void(*function)());
	void add_dependency(system_id id, component_id dependency);
	void run_systems();
}