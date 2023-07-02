#include "Script.h"

#include "EngineAPI/Components.h"
#include "EngineAPI/GameEntity.h"

namespace fabric::component
{
	namespace 
	{
		using script_registry = std::unordered_map<entity::component_id, detail::script_creator>;

		script_registry& registry()
		{
			static script_registry registry;
			return registry;
		}

		utl::vector<detail::script_ptr> all_scripts;
		
#ifdef USE_WITH_EDITOR
		utl::vector<std::string> script_names;
#endif
	}

	namespace detail
	{
		u8 register_script(entity::component_id id, script_creator func)
		{
			assert(func);
			registry().insert({id, func});

			return func && registry().contains(id);
		}

		script_creator get_script_creator(entity::component_id id)
		{
			auto script = registry().find(id);
			assert(script != registry().end() && script->first == id);

			return script->second;
		}

#ifdef USE_WITH_EDITOR
		u8 add_script_name(const char* name)
		{
			bool result = script_names.emplace_back(name).c_str();

			return result;
		}
#endif
	}

	void create_script()
	{
		
	}

	void update_scripts(float dt)
	{
		for (auto& script : all_scripts)
		{
			script->update(dt);
		}
	}

	void destroy_script()
	{
		
	}
}
