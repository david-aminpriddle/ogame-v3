import spaceDock from './assets/images/space dock.jpg';

export function App() {
  return (
    <div className="flex flex-row flex-1">
      <div className="flex flex-col m-3 gap-3">
        <div className="flex border rounded-lg p-3 gap-3 flex-col flex-1 justify-around">
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-gray-400 border-gray-400 cursor-pointer justify-between py-3">
              <div>Impulse</div>
              <div>off</div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-gray-400 border-gray-400 cursor-default justify-between py-3">
              <div>FTL</div>
              <div>off</div>
            </div>
          </div>
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-3">
              <div>Fuel</div>
              <div>
                E &#9608;&#9608;&#9608; F
              </div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-gray-400 border-gray-400 cursor-default justify-between py-3">
              <div>Velocity</div>
              <div>
                0m/s
              </div>
            </div>
          </div>
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col
              items-center text-gray-400 border-gray-400 cursor-default justify-between py-2">
              <div>Location</div>
              <div>Sector 0</div>
              <div>[0, 14, 2]</div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-3">
              <div>Hull</div>
              <div>
                100%
              </div>
            </div>
          </div>
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-3">
              <div>Shields</div>
              <div>
                100%
              </div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-center">
              Comm.
              <br />
              status
            </div>
          </div>
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-3">
              <div>Life supp.</div>
              <div>
                100%
              </div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-3">
              <div>Alerts</div>
              <div>
                None
              </div>
            </div>
          </div>
          <div className="flex flex-row gap-3">
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-gray-400 border-gray-400 cursor-default justify-between py-2">
              <div>Cargo</div>
              <div className="text-xs">
                450/450 SU
              </div>
              <div>
                100% full
              </div>
            </div>
            <div className="w-20 h-20 border rounded-lg text-sm small-caps flex flex-col items-center text-green-500 border-green-500 cursor-default justify-between py-2">
              <div>Crew</div>
              <div className="text-xs">
                450/450
              </div>
              <div>
                300 duty
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="flex flex-col flex-1 m-3 text-xs border rounded-lg p-3">
        <div style={{backgroundImage: `url('${spaceDock}')`}} className="flex flex-1 bg-center bg-contain bg-no-repeat">

        </div>
      </div>
    </div>
  );
}
