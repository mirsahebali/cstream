<script lang="ts" module>
	import * as v from 'valibot';

	const joinRoomSchema = v.object({
		id: v.nullable(
			v.pipe(
				v.number(),
				v.minValue(100000, 'invalid stream id'),
				v.maxValue(999999, 'invalid stream id')
			)
		)
	});
</script>

<script lang="ts">
	import { defaults, superForm } from 'sveltekit-superforms';
	import { valibot } from 'sveltekit-superforms/adapters';
	import { toast } from 'svelte-sonner';
	import * as Form from '$lib/components/ui/form/index.js';
	import { Input } from '$lib/components/ui/input/index.js';

	import * as Resizable from '$lib/components/ui/resizable/index';
	import { Button } from '$lib/components/ui/button';
	import { onMount } from 'svelte';
	import { getSignalRConnection } from '$lib/store';
	const signalRConnection = getSignalRConnection();

	let username = $state('');

	let joinedStream = $state(false);

	let remoteVideoRef: HTMLVideoElement | null = $state(null);

	const form = superForm(defaults(valibot(joinRoomSchema)), {
		validators: valibot(joinRoomSchema),
		SPA: true,
		onUpdate: async ({ form: f }) => {
			if (f.valid) {
				if (!username || username.length === 0) {
					toast.error('no user found');
					return;
				}

				localStorage.setItem('stream_id', String(f.data.id));

				signalRConnection.on(
					'JoinStreamData::' + username,
					async (sdp: RTCSessionDescriptionInit | null) => {
						if (sdp === null) {
							toast.error('error creating stream handshake');
							return;
						}

						const configuration = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
						rtcPeerConnection = new RTCPeerConnection(configuration);

						rtcPeerConnection.setRemoteDescription(new RTCSessionDescription(sdp));
						const answer = await rtcPeerConnection.createAnswer();
						await rtcPeerConnection.setLocalDescription(answer);

						await signalRConnection.invoke('JoinStreamAnswer', username, $formData.id, answer);
						toast.success('Joined Stream');

						joinedStream = true;
					}
				);

				await signalRConnection.start();
				await signalRConnection.invoke('JoinStream', f.data.id, username);
			} else {
				toast.error('Please fix the errors in the form.');
			}
		}
	});

	const { form: formData, enhance } = form;

	let rtcPeerConnection: RTCPeerConnection | null = null;

	onMount(() => {
		const localStorageUsername = localStorage.getItem('username');
		if (!localStorageUsername) {
			toast.error('no user found');
			return;
		}

		username = localStorageUsername;

		console.log('JoinStreamData::' + username, ' mounted');

		console.log('NewIceCandidate::' + username, ' mounted');
		signalRConnection.on(
			'NewIceCandidate::' + username,
			async (eventCandidate: RTCIceCandidate) => {
				try {
					await rtcPeerConnection?.addIceCandidate(eventCandidate);
				} catch (error) {
					console.error('Error receiveing ice candidcate', error);
				}
			}
		);

		rtcPeerConnection?.addEventListener('icecandidate', async (event) => {
			if (event.candidate) {
				signalRConnection.invoke('NewIceCandidate', $formData.id, event.candidate);
			}
		});
		// Listen for connectionstatechange on the local RTCPeerConnection
		rtcPeerConnection?.addEventListener('connectionstatechange', (event) => {
			if (rtcPeerConnection?.connectionState === 'connected') {
				toast.success('New peer connected');
			}
		});
		rtcPeerConnection?.addEventListener('track', async (event) => {
			console.log('Remote stream?');
			const [remoteStream] = event.streams;
			if (!remoteVideoRef) return;
			remoteVideoRef.srcObject = remoteStream;
			console.log('added remote stream');
		});
	});
</script>

<main class="mt-5 flex w-full flex-col items-center justify-center gap-4">
	<h1 class="text-2xl font-bold">CStream</h1>

	<h3 class=" rounded-lg border-2 bg-accent px-5 py-3 text-xl font-bold">
		Hello, <span class="underline"> {username}</span>
	</h3>

	{#if joinedStream}
		<div class="flex w-full flex-col items-center justify-center">
			<Resizable.PaneGroup direction="vertical" class="min-h-[50vh]  max-w-md rounded-lg border">
				<Resizable.Pane defaultSize={50}>
					<video bind:this={remoteVideoRef} id="remote-video" autoplay playsinline controls={false}>
						<track kind="captions" />
					</video>
				</Resizable.Pane>
				<Resizable.Handle />
			</Resizable.PaneGroup>
			<div class="my-4 flex gap-10">
				<Button class="items-center px-5 py-3">Exit Stream</Button>
			</div>
		</div>
	{:else}
		<div class="flex w-[80vw] flex-col items-center justify-center lg:w-[40vw]">
			<form
				method="POST"
				class="flex w-2/3 flex-col justify-center space-y-6 rounded-lg border bg-primary-foreground p-5"
				use:enhance
			>
				<Button href="/choice/host">Create Stream</Button>
				<span class="flex items-center justify-center text-xl font-bold">OR</span>
				<Form.Field {form} name="id">
					<Form.Control>
						{#snippet children({ props })}
							<Form.Label>Join Stream</Form.Label>
							<Input {...props} type="number" bind:value={$formData.id} />
						{/snippet}
					</Form.Control>
					<Form.Description>Enter stream ID</Form.Description>
					<Form.FieldErrors />
				</Form.Field>
				<Form.Button>Join Stream</Form.Button>
			</form>
		</div>
	{/if}
</main>
