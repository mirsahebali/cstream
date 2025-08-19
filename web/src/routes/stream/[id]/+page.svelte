<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import Toggle from './Toggle.svelte';
	import * as Resizable from '$lib/components/ui/resizable/index';
	import { onMount } from 'svelte';

	import * as signalR from '@microsoft/signalr';
	import { to } from '$lib/utils';

	let signalRConnection = new signalR.HubConnectionBuilder().withUrl(to('/stream')).build();

	let cameraList = $state<MediaDeviceInfo[] | null>();

	let localVideoRef: HTMLVideoElement | null = $state(null);
	let remoteVideoRef: HTMLVideoElement | null = $state(null);

	let rtcPeerConnection: RTCPeerConnection | null = $state(null);

	onMount(async () => {
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
		const configuration = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
		rtcPeerConnection = new RTCPeerConnection(configuration);

		signalRConnection.invoke('start_stream', []);
	}
</script>

<main class="mt-10 flex flex-col items-center justify-center gap-4">
	<h1 class="flex items-center justify-center text-4xl font-bold">CStream</h1>
	<Toggle />

	<hr />
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
			<Button class="items-center px-5 py-3">Start Stream</Button>
			<Button class="items-center px-5 py-3">End Stream</Button>
		</div>
	</div>
</main>
