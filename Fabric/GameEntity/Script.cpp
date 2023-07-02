#include "Script.h"

#include "EngineAPI/Components.h"
#include "EngineAPI/GameEntity.h"

namespace fabric::component
{
	u64 _scriptCounter = 0;
	namespace 
	{
		using script_list = utl::vector<detail::script_ptr>;
		using script_registry = std::unordered_map<size_t, detail::script_creator>;

		script_registry& registry()
		{
			static script_registry registry;
			return registry;
		}

		std::unordered_map<entity::entity_id, script_list> entity_scripts;
		
#ifdef USE_WITH_EDITOR
		utl::vector<std::string> script_names;
#endif
	}

	namespace detail
	{
		u8 register_script(size_t tag, script_creator func)
		{
			assert(func);
			registry().insert({tag, func});

			return func && registry().contains(tag);
		}

		script_creator get_script_creator(size_t tag)
		{
			auto script = registry().find(tag);
			assert(script != registry().end() && script->first == tag);

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
}
