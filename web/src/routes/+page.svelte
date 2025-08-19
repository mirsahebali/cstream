<script lang="ts" module>
	import * as v from 'valibot';

	const formSchema = v.object({
		username: v.pipe(
			v.string(),
			v.minLength(3, 'mininum 3 characters required'),
			v.maxLength(20, 'maximum 20 characters are allowed')
		)
	});
</script>

<script lang="ts">
	import { defaults, superForm } from 'sveltekit-superforms';
	import { valibot } from 'sveltekit-superforms/adapters';
	import { toast } from 'svelte-sonner';
	import * as Form from '$lib/components/ui/form/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { to } from '$lib/utils';
	import { goto } from '$app/navigation';

	const form = superForm(defaults(valibot(formSchema)), {
		validators: valibot(formSchema),
		SPA: true,
		onUpdate: async ({ form: f }) => {
			if (f.valid) {
				let res: Response | null;
				try {
					res = await fetch(to('/user?username=' + f.data.username));
				} catch (error) {
					toast.error('Error occoured while creating user');
					console.error('User create error: ', error);
					return;
				}

				if (!res) return;

				if (!res.ok || res.status !== 200) {
					toast.error('error occoured');
					return;
				}

				const data = await res.json();

				toast.success(`User ${f.data.username} created`);
				localStorage.setItem('username', f.data.username);
				goto('/stream/' + data.id);
			} else {
				toast.error('Please fix the errors in the form.');
			}
		}
	});

	const { form: formData, enhance } = form;
</script>

<main class="mt-5 flex w-full flex-col items-center justify-center gap-4">
	<h1 class="text-2xl font-bold">CStream</h1>
	<div class="flex w-[80vw] flex-col items-center justify-center lg:w-[40vw]">
		<form
			method="POST"
			class="flex w-2/3 flex-col justify-center space-y-6 rounded-lg border bg-primary-foreground p-5"
			use:enhance
		>
			<Form.Field {form} name="username">
				<Form.Control>
					{#snippet children({ props })}
						<Form.Label>Username</Form.Label>
						<Input {...props} bind:value={$formData.username} />
					{/snippet}
				</Form.Control>
				<Form.Description>This is your public display name.</Form.Description>
				<Form.FieldErrors />
			</Form.Field>
			<Form.Button>Submit</Form.Button>
		</form>
	</div>
</main>
