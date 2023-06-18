#pragma once

#include "Common/CommonHeaders.h"

#if !defined(SHIPPING)
namespace fabric::content
{
	bool load_game();
	void unload_game();
}
#endif