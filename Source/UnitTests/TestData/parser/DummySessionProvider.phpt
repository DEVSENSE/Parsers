<?php
use MediaWiki\Session\SessionProviderddd;
use MediaWiki\Session\SessionInfo;
use MediaWiki\Session\SessionBackend;
use MediaWiki\Session\UserInfo; 

/**
 * Dummy session provider
 *
 * An implementation of a session provider that doesn't actually do anything.
 */

	const ID = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa';

	function provideSessionInfo( WebRequest $request ) {
		return new SessionInfo( SessionInfo::MIN_PRIORITY, [
			'provider' => $this,
			'id' => self::ID,
			'persisted' => true,
			'userInfo' => UserInfo::newAnonymous(),
		] );
	}

<<<TEST>>>
<<<IGNORE>>>