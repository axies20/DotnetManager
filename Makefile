.PHONY: check test

check:
	bash -n bin/dotnet-manager install.sh uninstall.sh tests/test.sh
	@if command -v zsh >/dev/null 2>&1; then \
		zsh -n completions/_dotnet-manager; \
	else \
		echo "zsh is not installed; skipping completion syntax check"; \
	fi
	@if command -v shellcheck >/dev/null 2>&1; then \
		shellcheck bin/dotnet-manager install.sh uninstall.sh tests/test.sh; \
	else \
		echo "shellcheck is not installed; skipping static analysis"; \
	fi

test: check
	./tests/test.sh
