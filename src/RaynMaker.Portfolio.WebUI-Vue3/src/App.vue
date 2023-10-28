<script setup lang="ts">
	import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
	<header>
		<nav>
			<RouterLink to="/">Home</RouterLink>
			<RouterLink to="/about">About</RouterLink>
		</nav>
	</header>

	<RouterView />
</template>

<style scoped>
	header {
		line-height: 1.5;
		max-height: 100vh;
		display: flex;
		place-items: center;
		padding-right: calc(var(--section-gap) / 2);
	}

	nav {
		width: 100%;
		text-align: left;
		margin-left: -1rem;
		font-size: 1rem;
		padding: 1rem 0;
		margin-top: 1rem;
	}

	nav a.router-link-exact-active {
		color: var(--color-text);
	}

	nav a.router-link-exact-active:hover {
		background-color: transparent;
	}

	nav a {
		display: inline-block;
		padding: 0 1rem;
		border-left: 1px solid var(--color-border);
	}

	nav a:first-of-type {
		border: 0;
	}
</style>
