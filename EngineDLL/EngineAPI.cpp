#include "Common.h"

#include "Common/CommonHeaders.h"
#include "GameEntity/Script.h"

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <atlsafe.h>

using namespace fabric;

namespace
{
	HMODULE game{ nullptr };

	using _get_script_creator = fabric::script::detail::script_creator(*)(size_t);
	_get_script_creator get_script_creator{ nullptr };
	using _get_script_names = LPSAFEARRAY(*)(void);
	_get_script_names get_script_names{ nullptr };
}

EDITOR_INTERFACE u32 LoadGame(const char* path)
{
	if (game) return FALSE;
	game = LoadLibraryA(path);
	assert(game);

	get_script_creator = (_get_script_creator)GetProcAddress(game, "get_script_creator");
	get_script_names = (_get_script_names)GetProcAddress(game, "get_script_names");

	return (game && get_script_creator && get_script_names) ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadGame()
{
	if (!game) return FALSE;
	assert(game);
	int result{ FreeLibrary(game) };

	assert(result);
	game = nullptr;

	return TRUE;
}

EDITOR_INTERFACE script::detail::script_creator GetScriptCreator(const char* name)
{
	return (game && get_script_creator) ? get_script_creator(script::detail::string_hash()(name)) : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY GetScriptNames()
{
	return (game && get_script_names) ? get_script_names() : nullptr;
}