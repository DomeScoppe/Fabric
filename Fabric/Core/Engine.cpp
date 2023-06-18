#if !defined(SHIPPING)
#include "Content/ContentLoader.h"
#include "GameEntity/Script.h"

#include <thread>

bool engine_initialize()
{
	bool result{ fabric::content::load_game() };

	return result;
}

void engine_update()
{
	fabric::script::update(10.0f);
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void engine_shutdown()
{
	fabric::content::unload_game();
}
#endif
