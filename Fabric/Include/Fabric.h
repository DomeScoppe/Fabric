#pragma once

#include "Engine.h"

namespace fabric
{
	namespace entity
	{
		class entity
		{
		public:
			explicit entity() : _id{ id::invalid_id } {}
			explicit entity(ecs::entity_id id) : _id(id) {}

			ecs::entity_id get_id() { return _id; }

			template<typename Component>
			constexpr bool has_component()
			{
				return ecs::has_component(_id, ecs::get_component_id<Component>());
			}

			template<typename Component>
			constexpr void add_component()
			{
				ecs::component component
				{
					.owner = _id,
					.id = ecs::get_component_id<Component>(),
					.size = sizeof(Component)
				};

				ecs::add_component(component);
			}

			template<typename Component>
			constexpr void add_component(Component& component)
			{
				ecs::component comp
				{
					.owner = _id,
					.id = ecs::get_component_id<Component>(),
					.data = &component,
					.size = sizeof(Component)
				};

				ecs::add_component(comp);
			}

			template<typename Component>
			constexpr const Component& get_component()
			{
				Component* component = (Component*) ecs::get_component(_id, ecs::get_component_id<Component>());

				if (component)
					return *component;

				return Component();
			}

			template<typename Component>
			constexpr void remove_component()
			{
				ecs::remove_component(_id, ecs::get_component_id<Component>());
			}

		private:
			ecs::entity_id _id;
		};
	}

	namespace scene
	{
		template<typename Component>
		using Owner = typename utl::type_list<Component>;

		template<typename Head, typename... Tail>
		using Dependencies = typename utl::type_list<Head, Tail...>;

		template<typename Component, typename Head, typename... Tail>
		struct system
		{
			utl::type_list<Component> owner;
			utl::type_list<Head, Tail...> dependencies;
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

		entity::entity create_entity()
		{
			return entity::entity(ecs::create_entity());
		}

		void remove_entity(entity::entity e)
		{
			ecs::remove_entity(e.get_id());
		}

		template<typename Component, typename Head, typename... Tail> requires IsNotSame<Component, Head>
		void register_system(system<Component, Head, Tail...>& sys)
		{
			static ecs::system_id id = ecs::register_system(ecs::get_component_id<Owner<Component>>(), sys.function);
			ecs::add_dependency(id, ecs::get_component_id<Dependencies<Head>>());
			
				Owner<Component> component_ownership;
				Dependencies<Tail...> component_dependencies;
				system<Component, Tail...> component_system
				{
					.owner = component_ownership,
					.dependencies = component_dependencies,
					.function = sys.function
				};

				register_system(component_system);
		}

		template<typename Component, typename Head> requires IsNotSame<Component, Head>
		void register_system(system<Component, Head>& sys)
		{
			static ecs::system_id id = ecs::register_system(ecs::get_component_id<Owner<Component>>(), sys.function);
			ecs::add_dependency(id, ecs::get_component_id<Dependencies<Head>>());
		}

		void run_systems()
		{
			ecs::run_systems();
		}
	}
}
