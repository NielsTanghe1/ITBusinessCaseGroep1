namespace ITBusinessCase.Services;

public static class OrderNumberGenerator {
	private static int _current = 0;
	private static readonly object _lock = new();

	public static int Next() {
		lock (_lock) {
			_current++;
			return _current;
		}
	}
}
