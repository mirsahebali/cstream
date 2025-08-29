build-web:
	cd web && pnpm run build && cp -r build ../backend/wwwroot/

