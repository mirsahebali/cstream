<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import Toggle from './Toggle.svelte';
	import * as Resizable from '$lib/components/ui/resizable/index';
	import { onMount } from 'svelte';

	import { toast } from 'svelte-sonner';
	import { goto } from '$app/navigation';
	import { getSignalRConnection } from '$lib/store';

	let username = $state('');
	let streamID = $state<number | null>();
	let cameraList = $state<MediaDeviceInfo[] | null>();

	let signalRConnection = getSignalRConnection();

	let localVideoRef: HTMLVideoElement | null = $state(null);
	let remoteVideoRef: HTMLVideoElement | null = $state(null);

	let rtcPeerConnection: RTCPeerConnection | null = $state(null);

	onMount(async () => {
		const localStorageUsername = localStorage.getItem('username');
		if (!localStorageUsername) {
			await goto('/');
			return;
		}

		username = localStorageUsername;

		signalRConnection.on('SetData::' + username, (id: number) => {
			console.log('Recv any data?');
			streamID = id;
		});

		const openMediaDevices = async (contraints?: MediaStreamConstraints) => {
			return await navigator.mediaDevices.getUserMedia(contraints);
		};

		const getConnectedDevices = async (type: MediaDeviceKind) => {
			const devices = await navigator.mediaDevices.enumerateDevices();
			return devices.filter((device) => device.kind === type);
		};

		cameraList = await getConnectedDevices('videoinput');

		navigator.mediaDevices.addEventListener('devicechange', async () => {
			const newCameraList = await getConnectedDevices('videoinput');
			cameraList = newCameraList;
		});

		const stream = await openMediaDevices({
			video: { deviceId: cameraList[0]?.deviceId } as MediaTrackConstraints,
			audio: { echoCancellation: true }
		});

		if (!localVideoRef) return;

		localVideoRef.srcObject = stream;
	});

	async function startStream() {
		await signalRConnection.start();

		const configuration = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
		rtcPeerConnection = new RTCPeerConnection(configuration);
		const username = localStorage.getItem('username');
		if (!username) {
			toast.error('no user found');
			return;
		}

		const offer = await rtcPeerConnection.createOffer();

		await rtcPeerConnection.setLocalDescription(offer);

		await signalRConnection.invoke('StartStream', username, offer);

		rtcPeerConnection.addEventListener('icecandidate', async (event) => {
			if (event.candidate) {
				await signalRConnection.invoke('NewIceCandidate', event.candidate);
			}
		});
	}
</script>

<main class="mt-10 flex flex-col items-center justify-center gap-4">
	<h1 class="flex items-center justify-center text-4xl font-bold">CStream</h1>
	<Toggle />

	<hr />
	<h3 class=" rounded-lg border-2 bg-accent px-5 py-3 text-xl font-bold">
		Hello, <span class="underline"> {username}</span>
	</h3>
	<Button class="text-md"
		>Stream ID: <span class="animate-pulse font-bold"
			>{streamID ? streamID : 'Start stream to get ID'}</span
		></Button
	>
	<div class="flex w-full flex-col items-center justify-center">
		<Resizable.PaneGroup direction="vertical" class="min-h-[50vh]  max-w-md rounded-lg border">
			<Resizable.Pane defaultSize={50}>
				<video bind:this={localVideoRef} id="local-video" autoplay playsinline controls={false}>
					<track kind="captions" />
				</video>
			</Resizable.Pane>
			<Resizable.Handle withHandle />
			<Resizable.Pane defaultSize={50}>
				<video bind:this={remoteVideoRef} id="remote-video" autoplay playsinline controls={false}>
					<track kind="captions" />
				</video>
			</Resizable.Pane>
			<Resizable.Handle />
		</Resizable.PaneGroup>
		<div class="my-4 flex gap-10">
			<Button class="items-center px-5 py-3" onclick={startStream}>Start Stream</Button>
			<Button class="items-center px-5 py-3">End Stream</Button>
		</div>
	</div>
</main>
